//Components are imported asynchronously for webpack bundle optimisation

export default [
    //-- Dashboard Page -------------------------------------------------
    {
        path: '/',
        name: 'Dashboard',
        component: () => import ("../components/home/home.vue"),
        meta: { title: 'Dashboard' }
    },
    //--Users Pages-------------------------------------------------------------------
    {
        path: '/users',
        name: 'Users',
        component: () => import ('../components/admin/users.vue'),
        meta: { title: 'Users' }
    },
    //-- 404 Error page ---------------------------------------------------
    {
        path: '*', name: 'PageNotFound',
        component: () => import ("../components/pagenotfound/pagenotfound.vue"),
        meta: { title: 'Page not found' }
    }
];
