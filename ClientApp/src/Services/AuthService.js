import jwt_decode from "jwt-decode"

export class AuthService {
    static isLogged() {
        return localStorage.getItem("token") !== null;
    }

    static logOut() {
        try {
            localStorage.removeItem("token");
        } catch (e) {
            console.error(e);
        }
    }
}