import Vue from "vue";
import { Component } from "vue-property-decorator";
import { UserRole } from '../dataModels/users/userRole'

@Component
export default class UserContextMixin extends Vue {
    constructor() {
        super()
    }

    get myId(): string {
        return this.$store.state.userContext.userId
    }

    get isAdmin(): boolean {
        return this.$store.state.userContext.isInRole(UserRole.ADMINISTRATOR)
    }

    get isUser(): boolean {
        return this.$store.state.userContext.isInRole(UserRole.USER)
    }
}
