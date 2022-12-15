export class UserRole {
    public static readonly USER = "User"
    public static readonly ADMINISTRATOR = "Administrator"

    public static AllRoles = [
        UserRole.ADMINISTRATOR,
        UserRole.USER
    ]
}
