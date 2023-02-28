import { User } from "@js/dataModels/users/user";
import EntityApiClient from "@js/api/entityApiClient";
import { UserContext } from "@js/dataModels/users/userContext";
import { plainToInstance } from "class-transformer";

//Note, in addition to basic add/get/edit/delete operations we want to be able to reset user password.
//In order to do this we extend the base EntityApiClient class, adding new functionality.
export default class UserApiClient extends EntityApiClient<User, string>{

    constructor() {
        super(User, "Users", "v1");
    }

    public async resetPassword(loginId: string): Promise<void> {
        await this.put(`${loginId}/ResetPassword`);
    }

    public async unlock(userId: string): Promise<void> {
        await this.put(`${userId}/Unlock`);
    }

    public async whoAmI(): Promise<UserContext> {
        const data = await this.get("WhoAmI");
        return plainToInstance(UserContext, await data.json());
    }

    public async search(query: string): Promise<any> {
        const response = await this.get(`?query=${query}`);
        return await response.json();
    }
}