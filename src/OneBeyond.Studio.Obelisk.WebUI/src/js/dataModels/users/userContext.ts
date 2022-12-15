import { StringUtils } from "@js/util/stringUtils";

export class UserContext {
    public readonly userName: string = "";
    public readonly userId: string = "";
    public readonly userTypeId: string = "";
    public readonly userRoleId: string | null = null;

    get initials(): string {
        return StringUtils.getInitials(this.userName);
    }

    public isUserType(typeId: string): boolean {
        return !!this.userTypeId && this.userTypeId.toLowerCase() == typeId.toLowerCase();
    }

    public isInRole(roleId: string): boolean {
        return !!this.userRoleId && this.userRoleId.toLowerCase() == roleId.toLowerCase();
    }

    public isEmpty(): boolean {
        return this.userId === "";
    }
}