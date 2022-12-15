export class UserContext {
    public readonly userId: string = ""
    public readonly userTypeId: string | null = null
    public readonly userRoleId: string | null = null

    private constructor(userId: string, userTypeId: string, userRoleId: string) {
        this.userId = userId
        this.userTypeId = userTypeId
        this.userRoleId = userRoleId
    }

    public isUserType(typeId: string): boolean {
        return this.userTypeId != null && this.userTypeId.toLowerCase() == typeId.toLowerCase()
    }

    public isInRole(roleId: string): boolean {
        return this.userRoleId != null && this.userRoleId.toLowerCase() == roleId.toLowerCase()
    }

    public static FromJson(json: string): UserContext {

        const userData = JSON.parse(json)

        if (userData.UserId == null) {
            throw new Error("User Id is not found in the user context")
        }

        if (userData.UserTypeId == null) {
            throw new Error("User TypeId is not found in the user context")
        }

        return new UserContext(userData.UserId, userData.UserTypeId, userData.UserRoleId)
    }

    public static Empty(): UserContext {
        return new UserContext("","","")
    }
}
