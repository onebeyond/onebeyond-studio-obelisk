<template>
    <div>
        <div class="title-bar">
            <div class="row">
                <div class="col-5">
                    <h1 class="page-title">Dashboard</h1>
                    <p>Data.</p>
                </div>

                <div class="col-7 justify-content-end">
                    <div class="d-flex no-block justify-content-end">
                        <!--<a class="btn btn-primary" href="http://www.google.com">Action</a>-->
                    </div>
                </div>
            </div>
        </div>

        <div class="row">
            <div class="col-lg-6">
                <div class="card content-panel">
                    <div class="card-body">
                        <h2 class="card-title">
                            My Context
                        </h2>
                        <p>
                            My UserName:
                            <b>{{$store.state.userContext.userName}}</b>
                        </p>
                        <p>
                            My Id:
                            <b>{{$store.state.userContext.userId}}</b>
                        </p>
                        <p>
                            My Type:
                            <b>{{$store.state.userContext.userTypeId}}</b>
                        </p>
                        <p>
                            My Role:
                            <b>{{$store.state.userContext.userRoleId}}</b>
                        </p>
                        <p>
                            Am I Admin?:
                            <b>{{isAdmin}}</b>
                            <i>(see how to use UserContextMixin)</i>
                        </p>
                    </div>
                </div>
            </div>

            <div class="col-lg-6">
                <div class="card content-panel">
                    <div class="card-body">
                        <h2 class="card-title">
                            Data
                        </h2>Widget 2
                    </div>
                </div>
            </div>
        </div>

        <div class="card content-panel">
            <div class="card-body">
                <h2 class="card-title">
                    Data
                </h2>Widget 3
            </div>
        </div>
    </div>
</template>


<script lang="ts">
    import { Component, Mixins } from "vue-property-decorator";
    import UserContextMixin from "@js/mixins/userContextMixin";

    @Component({
        name: "Dashboard",
    })
    export default class Dashboard extends Mixins(UserContextMixin) {
        constructor() {
            super();
        }
    }
</script>
