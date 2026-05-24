/**
 * Jellyfin Web Globals
 */
const {Dashboard, ApiClient} = window;

const pluginId = 'eb5d7894-8eef-4b36-aa6f-5d124e828ce1';

document.querySelector('#TemplateConfigPage')
    .addEventListener('pageshow', function() {
        Dashboard.showLoadingMsg();

        document.querySelector('#btnAddTuner').addEventListener('click', function() {
            const tunerInfo = {
                Type: 'template_livetv',
                Url: 'template',
                FriendlyName: 'Template Tuner',
                TunerCount: 0
            };
            Dashboard.showLoadingMsg();
            ApiClient.ajax({
                type: 'POST',
                url: ApiClient.getUrl('LiveTv/TunerHosts'),
                data: JSON.stringify(tunerInfo),
                contentType: 'application/json'
            }).then(() => {
                Dashboard.hideLoadingMsg();
                Dashboard.alert('Tuner successfully added!');
            }).catch(() => Dashboard.hideLoadingMsg());
        });

        document.querySelector('#btnAddGuide').addEventListener('click', function() {
            const guideInfo = {
                Type: 'template_livetv',
                Id: 'template_guide',
                Name: 'Template Guide'
            };
            Dashboard.showLoadingMsg();
            ApiClient.ajax({
                type: 'POST',
                url: ApiClient.getUrl('LiveTv/ListingProviders'),
                data: JSON.stringify(guideInfo),
                contentType: 'application/json'
            }).then(() => {
                Dashboard.hideLoadingMsg();
                Dashboard.alert('Guide successfully added!');
            }).catch(() => Dashboard.hideLoadingMsg());
        });

        document.querySelector('#btnPing').addEventListener('click', function() {
            Dashboard.showLoadingMsg();
            ApiClient.ajax({
                type: 'GET',
                url: ApiClient.getUrl('MyPlugin/ping'),
                dataType: 'text'
            }).then((response) => {
                Dashboard.hideLoadingMsg();
                Dashboard.alert('Ping response: ' + response);
            }).catch(() => Dashboard.hideLoadingMsg());
        });

        ApiClient.getPluginConfiguration(pluginId).then(function (config) {
            document.querySelector('#Options').value = config.Options;
            document.querySelector('#AnInteger').value = config.AnInteger;
            document.querySelector('#TrueFalseSetting').checked = config.TrueFalseSetting;
            document.querySelector('#AString').value = config.AString;
            Dashboard.hideLoadingMsg();
        });
    });

document.querySelector('#TemplateConfigForm')
    .addEventListener('submit', function(e) {
    Dashboard.showLoadingMsg();
    ApiClient.getPluginConfiguration(pluginId).then(function (config) {
        config.Options = document.querySelector('#Options').value;
        config.AnInteger = document.querySelector('#AnInteger').value;
        config.TrueFalseSetting = document.querySelector('#TrueFalseSetting').checked;
        config.AString = document.querySelector('#AString').value;
        ApiClient.updatePluginConfiguration(pluginId, config).then(function (result) {
            Dashboard.processPluginConfigurationUpdateResult(result);
        });
    });

    e.preventDefault();
    return false;
});
