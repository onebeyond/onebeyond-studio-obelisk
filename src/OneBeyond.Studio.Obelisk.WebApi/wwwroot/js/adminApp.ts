const Vue = require("vue");
import store from "@js/stores/appStore";
import LocalSessionStorage from "@js/stores/localSessionStorage";
import { router } from "@js/vueConfig/config";
import { i18n } from '@js/localizations/lang';

// Call a first time when the App is created
// NOTE: This is needed here as the homepage loading does
// not involve VueResource (so the interceptor is not called)
LocalSessionStorage.updateLastServerRequestDate();


const adminAppInstance = new Vue({
    el: "#adminApp",
    router,
    store,
    i18n,
    components: {
        Event
    },
    methods: {
        isSelected(link) {
            return link === adminAppInstance.$route.name;
        }
    }
});