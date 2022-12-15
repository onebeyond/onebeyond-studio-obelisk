<template>
    <div>
        <div class="title-bar">
            <div class="row">
                <div class="col-5">
                    <h1 class="page-title">{{ $t('entityName.plural') }}</h1>
                </div>

                <div class="col-7 d-flex justify-content-end">
                    <div>
                        <button
                            id="createNewBtn"
                            class="btn btn-primary btn-sm"
                            v-if="isAdmin"
                            v-on:click="onAddEntityButtonClicked"
                        >
                            <i class="fas fa-plus"></i>
                            {{ $t('button.createNew') }}
                        </button>
                    </div>
                </div>
            </div>
        </div>

        <div class="card content-panel">
            <div class="card-body">
                <v-server-table
                    id="mainTable"
                    :columns="entityGrid.columns"
                    :options="entityGrid.options"
                    ref="entityGrid"
                >
                    <div slot="isActive" slot-scope="props">{{props.row.isActive | showYesNo}}</div>
                    <div
                        slot="isLockedOut"
                        slot-scope="props"
                        style="width:100%;text-align:center; color:red;"
                    >
                        <span v-if="props.row.isLockedOut">{{ $t('message.lockedOut') }}</span>
                    </div>
                    <div slot="edit" slot-scope="props" class="actions-cell">
                        <button
                            class="btn btn-link btn-sm"
                            @click="onEditEntityButtonClicked(props.row.id)"
                        >
                            <i class="far fa-edit"></i>
                            {{ $t('button.edit') }}
                        </button>
                    </div>

                    <div slot="mobile" slot-scope="props">
                        <p>
                            <b>{{ $t('entityColumn.name') }}</b>
                            {{props.row.name}}
                        </p>
                        <p>
                            <b>{{ $t('entityColumn.emailAddress') }}</b>
                            {{props.row.email}}
                        </p>
                        <p>
                            <b>{{ $t('entityColumn.userName') }}</b>
                            {{props.row.userName}}
                        </p>
                        <p>
                            <b>{{ $t('entityColumn.roleId') }}</b>
                            {{props.row.roleId}}
                        </p>
                        <p>
                            <b>{{ $t('entityColumn.isActive') }}</b>
                            {{props.row.isActive}}
                        </p>
                        <button
                            class="btn btn-primary btn-sm"
                            @click="onEditEntityButtonClicked(props.row.id)"
                        >{{ $t('button.edit') }}</button>
                    </div>
                </v-server-table>
            </div>
        </div>

        <!--add/edit modal-->
        <v-modalPopup
            @close="closeEntityModal"
            :visible="showEntity"
            :title="$t('message.addEditUserTitle')"
        >
            <template slot="content">
                <div class="form-group row">
                    <label
                        for="emailInput"
                        class="col-sm-3 col-form-label"
                    >{{ $t('entityColumn.emailAddress') }}</label>
                    <div class="col-sm-9">
                        <input
                            id="emailInput"
                            type="email"
                            v-model="entity.email"
                            name="email"
                            v-validate="'required|email|max:150'"
                            :class="{'form-control': true, 'is-invalid': errors.has('email') }"
                            :data-vv-as="$t('entityColumn.emailAddress')"
                        />
                        <div
                            class="invalid-feedback"
                            v-show="errors.has('email')"
                        >{{ errors.first('email') }}</div>
                    </div>
                </div>

                <div class="form-group row">
                    <label
                        for="userNameInput"
                        class="col-sm-3 col-form-label"
                    >{{ $t('entityColumn.userName') }}</label>
                    <div class="col-sm-9">
                        <input
                            id="userNameInput"
                            type="text"
                            v-model="entity.userName"
                            name="userName"
                            v-validate="'required|max:150'"
                            :class="{'form-control': true, 'is-invalid': errors.has('userName') }"
                            :data-vv-as="$t('entityColumn.userName')"
                        />
                        <div
                            class="invalid-feedback"
                            v-show="errors.has('userName')"
                        >{{ errors.first('userName') }}</div>
                    </div>
                </div>

                <div class="form-group row" v-show="!isMe">
                    <label
                        for="roleSelect"
                        class="col-sm-3 col-form-label"
                    >{{ $t('entityColumn.roleId') }}</label>
                    <div class="col-sm-9">
                        <select
                            id="roleSelect"
                            v-model="entity.roleId"
                            name="roleID"
                            v-validate="'required'"
                            :class="{'form-control': true, 'is-invalid': errors.has('roleID') }"
                            :data-vv-as="$t('entityColumn.roleId')"
                        >
                            <option :value="null">{{ $t('placeholder.select') }}</option>
                            <option
                                v-for="role in roles"
                                v-bind:value="role"
                                v-bind:key="role"
                            >{{ role }}</option>
                        </select>
                        <div
                            class="invalid-feedback"
                            v-show="errors.has('roleID')"
                        >{{ errors.first('roleID') }}</div>
                    </div>
                </div>

                <div class="form-group row" v-show="!entity.isNew">
                    <div class="col-sm-9 offset-sm-3">
                        <div class="form-check">
                            <input
                                id="activeCheck"
                                type="checkbox"
                                v-model="entity.isActive"
                                name="active"
                                class="form-check-input"
                                :data-vv-as="$t('entityColumn.isActive')"
                            />
                            <label
                                class="form-check-label"
                                for="activeCheck"
                            >{{ $t('entityColumn.isActive') }}?</label>
                        </div>
                    </div>
                </div>
            </template>

            <template slot="footer">
                <button
                    v-if="!entity.isNew && entity.isLockedOut"
                    id="unlockBtn"
                    type="button"
                    class="btn btn-secondary"
                    v-on:click="unlock"
                >{{ $t('button.unlock') }}</button>
                <button
                    v-if="!entity.isNew"
                    id="resetPasswordBtn"
                    type="button"
                    class="btn btn-secondary"
                    v-on:click="resetPassword"
                >{{ $t('button.resetPassword') }}</button>
                <button
                    id="closePopupBtn"
                    type="button"
                    class="btn btn-secondary"
                    v-on:click="closeEntityModal"
                >{{ $t('button.close') }}</button>
                <el-button
                    id="confirmSaveBtn"
                    type="button"
                    class="btn btn-primary"
                    :disabled="errors.any()"
                    v-on:click="saveEntity"
                    :loading="isSaving"
                >{{ $t('button.save') }}</el-button>
            </template>
        </v-modalPopup>

        <!--alert modal-->
        <v-modalPopup
            :bus="this"
            :namespace="'alertModal'"
            :visible="alertVisible"
            @close="hideAlert"
        ></v-modalPopup>
    </div>
</template>

<script lang="ts">
    import { Component, Mixins } from "vue-property-decorator";
    import UserContextMixin from "@js/mixins/userContextMixin";
    import EntityCrudMixin from "@js/entityCrud/entityCrudMixin";
    import { EntityGridAction } from "@js/grids/entityGrid";
    import { VueEntityGrid } from "@js/grids/vueServerGrid/vueEntityGrid";
    import userDictionary from "@js/localizations/resources/components/users";

    import { EntityBuilder } from "@js/dataModels/entity";
    import { User } from "@js/dataModels/users/user";
    import { UserRole } from "@js/dataModels/users/userRole";

    import UserApiClient from "@js/api/users/userApiClient";

    import { Button } from "element-ui";

    @Component({
        name: "Users",
        components: {
            "el-button": Button
        },
        i18n: {
            messages: userDictionary
        }
    })
    export default class Users extends Mixins<EntityCrudMixin<User, string, VueEntityGrid>, UserContextMixin>(EntityCrudMixin, UserContextMixin) {

        entityApiClient!: UserApiClient;

        roles: any = UserRole.AllRoles;

        constructor() {
            super();
        }

        //Note, there are two basic parameters we need to provide for each class that is using EntityCrudMixin:

        //Entity builder, this is actually a class, which constructor will be used to create a new instance of the entity
        public provideEntityBuilder(): EntityBuilder<User, string> {
            return User;
        }

        // Entity Api Client, which is the class responsible to perform the HTTP requests against the server
        public provideEntityApiClient(): UserApiClient {
            return new UserApiClient();
        }

        public provideGrid(): VueEntityGrid {
            const entityGrid = new VueEntityGrid([
                "mobile",
                "email",
                "userName",
                "roleId",
                "isActive",
                "isLockedOut",
                "edit"
            ]);

            entityGrid
                .setCustomHeaders({
                    email: this.$t('entityColumn.emailAddress'),
                    userName: this.$t('entityColumn.userName'),
                    roleId: this.$t('entityColumn.roleId'),
                    isActive: this.$t('entityColumn.isActive'),
                    isLockedOut: this.$t('entityColumn.isLockedOut'),
                    edit: ''
                })
                .setDefaultSortOrder({ column: "userName", ascending: true })
                .excludeColumnsFromFiltering(['isActive', 'isLockedOut', 'edit'])
                .excludeColumnsFromSorting(['edit']);

            entityGrid.options.filterByColumn = true;
            return entityGrid;
        }

        public get isMe(): boolean {
            return this.entity.id == this.myId;
        }

        public async resetPassword(): Promise<void> {
            try {
                await this.entityApiClient.resetPassword(this.entity.loginId);
                this.showEntity = false;
            }
            catch (e) {
                this.onError(e);
            }
        }

        public async unlock(): Promise<void> {
            try {
                this.entityGrid.rememberCurrentPageBeforeGridAction(EntityGridAction.EntityEdit);
                await this.entityApiClient.unlock(this.entity.id);
                this.showEntity = false;
                this.entityGrid.restoreCurrentPage();
            }
            catch (e) {
                this.onError(e);
            }
        }
    }
</script>