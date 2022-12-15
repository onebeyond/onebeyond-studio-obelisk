const Vue = require("vue");
import store from "@js/stores/appStore";
import LocalSessionStorage from "@js/stores/localSessionStorage";
import { adminRouter } from "@js/routes/adminRoutes";
import { i18n } from '@js/localizations/lang';
import AppConfiguration from "./vueConfig/appConfiguration";

// NOTE: cannot use async / await on the top level with the current ES module version
new AppConfiguration(
    false, // Enable to see what variables are being set
    () => {
        // Call a first time when the App is created
        // NOTE: This is needed here as the homepage loading does
        // not involve VueResource (so the interceptor is not called)
        LocalSessionStorage.updateLastServerRequestDate();

        const adminAppInstance = new Vue({
            el: "#adminApp",
            router: adminRouter,
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
    })
    .setup();
