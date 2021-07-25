import jwt_decode from "jwt-decode"

export class AuthService {
    static isLogged() {
        if (localStorage.getItem("token") === null) return false;
        return jwt_decode(localStorage.getItem("token"));
    }

    static logOut() {
        try {
            localStorage.removeItem("token");
        } catch (e) {
            console.error(e);
        }
    }
    
    static isCurrentUserRoleInList(...roleTypes) {
        try {
            let result = true;
            const decodedJwtToken = jwt_decode(localStorage.getItem("token"));
            for (const roleType in roleTypes) {
                if (roleType !== decodedJwtToken.role) result = false;
            }
            return result;
        }
        catch (error) {
            console.error(error);
            return false;
        }
    }
}