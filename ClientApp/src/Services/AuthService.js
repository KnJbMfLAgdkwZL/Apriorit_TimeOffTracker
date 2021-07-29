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
    
    static isCurrentUserRoleEquals(role) {
        try {
            const decodedJwtToken = jwt_decode(localStorage.getItem("token"));
            if (String(role) === String(decodedJwtToken.role)) return true;
        }
        catch (error) {
            console.error(error);
            return false;
        }
    }
    
    static getCurrentUserRole() {
        if (localStorage.getItem("token") === null) return undefined;

        try {
            const decodedJwtToken = jwt_decode(localStorage.getItem("token"));
            return String(decodedJwtToken.role);
        }
        catch (error) {
            console.error(error);
            return undefined;
        }
    }
}