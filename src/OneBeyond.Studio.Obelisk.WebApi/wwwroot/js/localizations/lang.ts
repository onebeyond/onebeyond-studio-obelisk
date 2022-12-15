import Vue from 'vue';
import VueI18n from 'vue-i18n';
import sharedLocalisations from "@js/localizations/resources/shared";
import dateTimeFormats from "@js/localizations/resources/dateTimeFormats";
import { i18nConfig } from './i18nConfig';
import LocalAppStorage from "@js/stores/localAppStorage";

Vue.use(VueI18n);

// Create VueI18n instance with options
const i18n = new VueI18n({
    locale: LocalAppStorage.getValueForKey("currentLocale") ?? "en", // set locale
    messages: sharedLocalisations, // set locale messages
    dateTimeFormats, // set datetime formats
    silentFallbackWarn: true
})

i18nConfig.use(i18n.locale);

export { i18n }