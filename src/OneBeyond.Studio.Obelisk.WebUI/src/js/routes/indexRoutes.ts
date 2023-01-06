const Vue = require("vue"); // eslint-disable-line @typescript-eslint/no-var-requires
import Router from "vue-router";

Vue.use(Router);

const indexRouter = new Router({
    mode: 'history',
    routes: [
        // //-- Home Page -------------------------------------------------
        {
            path: '/',
            name: 'Home',
            component: () => import("@components/home/home.vue"),
            meta: { title: 'Home' },
            // beforeEnter(_to, _from, _next) {
            //     window.location.href = `${(window as any).location.origin}/admin/`;
            // }
        },
        //-- 404 Error page ---------------------------------------------------
        {
            path: '*', name: 'PageNotFound',
            component: () => import("@components/pagenotfound/pagenotfound.vue"),
            meta: { title: 'Page not found' }
        }
    ],
    linkActiveClass: "active"
});

const applicationName = document.getElementsByTagName("title")[0].innerHTML;
indexRouter.beforeEach((to, _from, next) => {
    document.title = to.meta.title + " - " + applicationName;
    document.body.className = `page-${to.name}`;
    window.scrollTo(0, 0);

    next();
});

export { indexRouter }

