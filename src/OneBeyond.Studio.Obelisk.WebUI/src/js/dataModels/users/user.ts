import { GuidEntity } from '../entity'

export class User extends GuidEntity {
    loginId = "";
    email = "";
    userName = "";
    typeId: string | null = null;
    roleId: string | null = null;
    isActive = false;
    isLockedOut = false;
}