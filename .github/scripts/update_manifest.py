import json
import os
import yaml
from datetime import datetime

def main():
    # Paths are relative to the root of the workspace where the action runs
    manifest_path = 'manifest-repo'
    plugin_path = 'plugin-repo'

    # --- Determine target filename based on prerelease status ---
    # The env var is passed as a string 'true' or 'false'
    is_prerelease = os.environ.get('IS_PRERELEASE', 'false').lower() == 'true'
    target_filename = 'manifest-prerelease.json' if is_prerelease else 'manifest.json'
    manifest_file = os.path.join(manifest_path, target_filename)

    print(f"Release type detected: {'Prerelease' if is_prerelease else 'Stable'}")
    print(f"Updating file: {target_filename}")

    build_file = os.path.join(plugin_path, 'build.yaml')

    # Ensure PLUGIN_GUID is available
    plugin_guid = os.environ.get('PLUGIN_GUID')
    if not plugin_guid:
        raise ValueError("Environment variable PLUGIN_GUID is missing")

    # --- Read build.yaml for plugin metadata ---
    print(f"Reading build config from: {build_file}")
    with open(build_file, 'r') as f:
        build_config = yaml.safe_load(f)

    # --- Info from release event and previous job ---
    # GITHUB_REF_NAME is automatically provided by GitHub Actions
    tag_name = os.environ.get('GITHUB_REF_NAME')
    if not tag_name:
        raise ValueError("GITHUB_REF_NAME is missing")

    version_str = build_config.get('version', tag_name.lstrip('v'))
    release_body = os.environ.get('RELEASE_BODY', 'See release notes.')
    repo_slug = os.environ.get('GITHUB_REPOSITORY')
    artifact_name = os.environ.get('ARTIFACT_NAME')

    if not artifact_name:
        raise ValueError("ARTIFACT_NAME environment variable is missing")

    # Read checksum from file (expected in the workspace root)
    checksum_file = 'checksum.txt'
    if not os.path.exists(checksum_file):
        raise FileNotFoundError(f"Checksum file not found at {checksum_file}")

    with open(checksum_file) as f:
        checksum = f.read().strip()

    # Construct the source URL
    source_url = f"https://github.com/{repo_slug}/releases/download/{tag_name}/{artifact_name}"

    # Create the new version entry
    new_version_entry = {
        "version": version_str,
        "changelog": release_body,
        "timestamp": datetime.utcnow().strftime('%Y-%m-%dT%H:%M:%SZ'),
        "sourceUrl": source_url,
        "checksum": checksum,
        "targetAbi": build_config.get('targetAbi', 'unknown')
    }

    # Read the existing manifest or initialize an empty list
    manifest_data = []
    try:
        if os.path.exists(manifest_file):
            with open(manifest_file, 'r') as f:
                manifest_data = json.load(f)
        else:
            print(f"'{manifest_file}' does not exist. A new one will be created.")
    except json.JSONDecodeError:
        print(f"'{manifest_file}' is invalid JSON. A new one will be created.")
        manifest_data = []

    # Find plugin by GUID and update/create it
    plugin_found = False
    for plugin in manifest_data:
        if plugin.get('guid') == plugin_guid:
            # Update top-level metadata from build.yaml if plugin exists
            plugin['category'] = build_config.get('category', 'General')
            plugin['name'] = build_config.get('name', 'Unknown Plugin')
            plugin['description'] = build_config.get('description', '')
            plugin['owner'] = build_config.get('owner', 'Unknown Owner')
            plugin['overview'] = build_config.get('overview', '')

            # Prepend the new version to the existing plugin's versions list
            plugin['versions'].insert(0, new_version_entry)
            plugin_found = True
            break

    if not plugin_found:
        print(f"Plugin with GUID {plugin_guid} not found. Creating new entry.")
        new_plugin_entry_obj = {
            "category": build_config.get('category', 'General'),
            "guid": plugin_guid,
            "name": build_config.get('name', 'Unknown Plugin'),
            "description": build_config.get('description', ''),
            "owner": build_config.get('owner', 'Unknown Owner'),
            "overview": build_config.get('overview', ''),
            "versions": [new_version_entry]
        }
        manifest_data.append(new_plugin_entry_obj)

    # Write the updated data back to the manifest file
    with open(manifest_file, 'w') as f:
        json.dump(manifest_data, f, indent=4)
        print(f"Successfully wrote to {manifest_file}")

    # Set output for the version used in the manifest
    # In python, we append to the file defined in GITHUB_OUTPUT env var
    print(f"manifest_version={version_str}")
    if 'GITHUB_OUTPUT' in os.environ:
        with open(os.environ['GITHUB_OUTPUT'], 'a') as f:
            f.write(f"manifest_version={version_str}\n")

if __name__ == "__main__":
    main()
