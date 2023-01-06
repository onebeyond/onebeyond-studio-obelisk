import { PhoneValidator, ComplexPasswordValidator, DateRangeValidator } from "@js/util/custom-validator";
import { showYesNo, shortDate, longDateTime, monthYearDate, currency, sizeInKb } from "@js/util/filters";
import 'reflect-metadata'; // Needed to make "class-transformer" work properly
import VeeValidate from "vee-validate";
import "@js/util/stringExtensions";
import { ServerTable, ClientTable } from "vue-tables-2";
import locale from "element-ui/lib/locale";
import en from "element-ui/lib/locale/lang/en";

import { vueTableLocale } from "@js/localizations/thirdParty/vueTable";
//Global components
//NOTE: Only add here if truly required globally, doing so inflates the bundle size
import SessionTimeoutComponent from "@components/dcslcomponents/sessionTimeout.vue";
import UserContextSetter from "@components/dcslcomponents/userContextSetter.vue";
import ModalPopup from "@components/util/modalPopup.vue";

//Language selector for VueI18n
import LanguageSelector from "@components/dcslcomponents/languageSelector.vue";
import WebApiClient from "@js/api/webApiClient";
import Configuration from "@js/configuration/configuration";
import LocalSessionStorage from "@js/stores/localSessionStorage";

const Vue = require("vue");

export default class AppConfiguration {
    private enableDebugLogging = false;
    private readonly vueInstanceInit: Function;

    constructor(enableDebugLogging: boolean, vueInstanceInit: Function) {
        this.enableDebugLogging = enableDebugLogging;
        this.vueInstanceInit = vueInstanceInit;
    }

    public setup(): Promise<void> {
        this.log("App", `window.env = ${(window as any).env}`);
        return Configuration.load((window as any).env, () => this.onSettingsLoaded());
    }

    private onSettingsLoaded(): void {
        // We can now access the whole configuration object as it has been fully loaded
        this.inspect(Configuration.appSettings);

        this.setupVueVariables();
        this.registerPlugins();
        this.registerGlobalVueComponents();
        this.registerGlobalFilters();
        this.vueInstanceInit.call(this);
    }

    private setupVueVariables(): void {
        //custom globals on the vue instance
        Vue.prototype.$sessionTimeoutInMinutes = Configuration.appSettings.sessionTimeoutInMinutes || 60;
        Vue.prototype.$rootPath = (window as any).location.origin;

        this.setWebApiBaseUrl();

        Vue.prototype.$buildNumber = process.env.BUILD_NUMBER;
        Vue.prototype.$buildDate = process.env.BUILD_DATE;
        this.log("App", `Vue.prototype.$buildNumber = ${Vue.prototype.$buildNumber}`);
        this.log("App", `Vue.prototype.$buildDate = ${Vue.prototype.$buildDate}`);

        const env = process.env.NODE_ENV || 'development';

        //Disable devtools in production
        Vue.config.devtools = env == "development";
    }

    private setWebApiBaseUrl(): void {
        let customApiUrl: string | null = null;
        if (Configuration.appSettings.allowApiUrlOverrideFromDevTools) {
            // Allow API Url to be overridden from console
            (window as any).setApiUrl = (url: string) => {
                LocalSessionStorage.setCustomApiUrl(url);
                location.reload();
            }
        
            (window as any).resetApiUrl = () => {
                LocalSessionStorage.setCustomApiUrl("");
                location.reload();
            };

            customApiUrl = LocalSessionStorage.getCustomApiUrl();
        }

        WebApiClient.WebApiRoot = customApiUrl || Configuration.appSettings.apiUrl || "";
        this.log("App", `WebApiRoot = ${WebApiClient.WebApiRoot}`);
    }

    private registerPlugins(): void {
        Vue.use(VeeValidate);
        locale.use(en);

        //Validator extensions
        VeeValidate.Validator.extend("phone", PhoneValidator);
        VeeValidate.Validator.extend("complexPassword", ComplexPasswordValidator);
        VeeValidate.Validator.extend("dateRange", DateRangeValidator);

        // vue-tables
        Vue.use(ServerTable, vueTableLocale, false, 'bootstrap4');
        Vue.use(ClientTable, vueTableLocale, false, 'bootstrap4');
    }

    // NOTE: You should keep this to the minimum!
    private registerGlobalVueComponents(): void {
        Vue.component("session-timeout", SessionTimeoutComponent);
        Vue.component("user-context", UserContextSetter);
        Vue.component("v-modalPopup", ModalPopup);
        Vue.component("language-selector", LanguageSelector);
    }

    private registerGlobalFilters(): void {
        Vue.filter("showYesNo", showYesNo);
        Vue.filter("shortDate", shortDate);
        Vue.filter("longDateTime", longDateTime);
        Vue.filter("monthYearDate", monthYearDate);
        Vue.filter("currency", currency);
        Vue.filter('sizeInKb', sizeInKb);
    }

    private inspect(object: any): void {
        if (this.enableDebugLogging) {
            console.log(object);
        }
    }

    private log(context: string, message: string): void {
        if (this.enableDebugLogging) {
            console.log(`[AppConfiguration][DEBUG][${context}] ${message}`);
        }
    }
}