const cookie = require('vue-cookies');
import { elementUILocale } from '@js/localizations/thirdParty/elementUI';
import { veeValidateLocale } from '@js/localizations/thirdParty/veeValidate';
import { syncfusionLocale } from '@js/localizations/thirdParty/syncfusion';
import LocalAppStorage from "../stores/localAppStorage";

const i18nConfig = {
    use(currentLocale: string) {
        if (currentLocale !== LocalAppStorage.getValueForKey("currentLocale")) {

            LocalAppStorage.setValueForKey("currentLocale", currentLocale);

            // Set the locale cookie for the ASP pages (accepts ISO language codes, e.g. 'es' or 'en-GB')
            const cookieValue = "c=" + currentLocale + "|uic=" + currentLocale;
            cookie.set('.AspNetCore.Culture', cookieValue, "1y", "/");

            // Set the locale for Element-UI, Syncfusion and Vee-Validate
            elementUILocale.use(currentLocale);
            syncfusionLocale.use(currentLocale);
            veeValidateLocale.use(currentLocale);
        }
    }
}

export { i18nConfig };