import { createApp } from 'vue'
import './style.css'
import App from './App.vue'

let appInstance = null;

function mountApp(container) {
    if (!appInstance) {
        appInstance = createApp(App);
        appInstance.mount(container);
    }
}

function unmountApp() {
    if (appInstance) {
        appInstance.unmount();
        appInstance = null;
    }
}

const pageElementId = 'TemplateVueJSPage';
const page = document.getElementById(pageElementId);
const isStandalone = import.meta.env.DEV || !window.Dashboard;

if (page) {
    if (isStandalone) {
        const container = page.querySelector('#app');
        if (container) {
            mountApp(container);
        }
    } else {
        page.addEventListener('pageshow', () => {
            const container = page.querySelector('#app');
            if (container) {
                mountApp(container);
            }
        });

        page.addEventListener('pagehide', () => {
            unmountApp();
        });
    }
}

