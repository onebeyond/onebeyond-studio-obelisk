import { PhoneValidator, ComplexPasswordValidator, DateRangeValidator } from "@js/util/custom-validator";
import { showYesNo, shortDate, longDateTime, monthYearDate, currency, sizeInKb } from "@js/util/filters";
import VeeValidate from "vee-validate";
import "@js/util/stringExtensions";
import 'reflect-metadata'; // Needed to make "class-transformer" work properly
import Router from "vue-router";
import routes from "@js/routes";
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

const Vue = require("vue");
const cookie = require('vue-cookies');
const env = process.env.NODE_ENV || 'development'
const applicationName = document.getElementsByTagName("title")[0].innerHTML;
const rootPath = (window as any).BaseUrl;
const router = new Router({ base: rootPath, routes: routes, linkActiveClass: "active" });

//Disable devtools in production
Vue.config.devtools = env == "development";

//plugins
Vue.use(Router);
Vue.use(VeeValidate);
Vue.use(cookie);

locale.use(en);

//custom globals on the vue instance
Vue.prototype.$rootPath = rootPath;
Vue.prototype.$rootApiPath = rootPath + "api/";

//global component registration
Vue.component("session-timeout", SessionTimeoutComponent);
Vue.component("user-context", UserContextSetter);
Vue.component("v-modalPopup", ModalPopup);
Vue.component("language-selector", LanguageSelector);

router.beforeEach((to, from, next) => {
    document.title = to.meta.title + " - " + applicationName;
    document.body.className = `page-${to.name}`;
    window.scrollTo(0, 0);
    next();
});

//global filters
Vue.filter("showYesNo", showYesNo);
Vue.filter("shortDate", shortDate);
Vue.filter("longDateTime", longDateTime);
Vue.filter("monthYearDate", monthYearDate);
Vue.filter("currency", currency);
Vue.filter('sizeInKb', sizeInKb);

//Validator extensions
VeeValidate.Validator.extend("phone", PhoneValidator);
VeeValidate.Validator.extend("complexPassword", ComplexPasswordValidator);
VeeValidate.Validator.extend("dateRange", DateRangeValidator);

Vue.use(ServerTable, vueTableLocale, false, 'bootstrap4');
Vue.use(ClientTable, vueTableLocale, false, 'bootstrap4');

export { router, rootPath, cookie }