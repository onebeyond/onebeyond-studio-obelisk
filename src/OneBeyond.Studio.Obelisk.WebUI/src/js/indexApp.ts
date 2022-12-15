const Vue = require("vue");
import { i18n } from '@js/localizations/lang';
import { indexRouter } from "@js/routes/indexRoutes";
import AppConfiguration from "./vueConfig/appConfiguration";

// NOTE: cannot use async / await on the top level with the current ES module version
new AppConfiguration(
    false, // Enable to see what variables are being set
    () => {
        const publicAppInstance = new Vue({
            el: "#indexApp",
            router: indexRouter,
            i18n,
            components: {
                Event
            },
            methods: {
                isSelected(link) {
                    return link === publicAppInstance.$route.name;
                }
            }
        })
    })
    .setup();