const Vue = require("vue"); // eslint-disable-line @typescript-eslint/no-var-requires
import Router from "vue-router";

Vue.use(Router);

const authRouter = new Router({
    mode: 'history',
    base: 'auth',
    routes: [
        //-- Sign in Page -------------------------------------------------
        {
            path: '/',
            name: 'SignIn',
            component: () => import("@components/auth/signIn.vue"),
            meta: { title: 'Sign in' }
        },
        //-- 404 Error page ---------------------------------------------------
        {
            path: '*', name: 'PageNotFound',
            component: () => import("@components/pagenotfound/pagenotfound.vue"),
            meta: { title: 'Page not found' }
        }],
    linkActiveClass: "active"
});

const applicationName = document.getElementsByTagName("title")[0].innerHTML;
authRouter.beforeEach((to, _from, next) => {
    document.title = to.meta.title + " - " + applicationName;
    document.body.className = `page-${to.name}`;
    window.scrollTo(0, 0);

    next();
});

export { authRouter }

