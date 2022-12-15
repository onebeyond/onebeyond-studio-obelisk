const Vue = require("vue");
import Router from "vue-router";
import LocalSessionStorage from "@js/stores/localSessionStorage";

Vue.use(Router);

const adminRouter = new Router({
    mode: 'history',
    base: 'admin',
    routes: [
        //-- Dashboard Page -------------------------------------------------
        {
            path: '/',
            name: 'Dashboard',
            component: () => import("@components/dashboard/dashboard.vue"),
            meta: { title: 'Dashboard' }
        },
        //--Users Pages-------------------------------------------------------------------
        {
            path: '/users',
            name: 'Users',
            component: () => import("@components/admin/users.vue"),
            meta: { title: 'Users' }
        },
        //-- 404 Error page ---------------------------------------------------
        {
            path: '*', 
            name: 'PageNotFound',
            component: () => import("@components/pagenotfound/pagenotfound.vue"),
            meta: { title: 'Page not found' }
        },
        //-- 404 Error page ---------------------------------------------------
        {
            path: '/notfound',
            name: 'PageNotFoundPage',
            component: () => import("@components/pagenotfound/pagenotfound.vue"),
            meta: { title: 'Page not found' }
        }],
    linkActiveClass: "active"
});

const applicationName = document.getElementsByTagName("title")[0].innerHTML;
adminRouter.beforeEach((to, _from, next) => {
    document.title = to.meta.title + " - " + applicationName;
    document.body.className = `page-${to.name}`;
    window.scrollTo(0, 0);

    if (LocalSessionStorage.isUserAuthenticated()) {
        next();
    } else {
        window.location.href = `${(window as any).location.origin}/auth/`;
    }
});

export { adminRouter }
