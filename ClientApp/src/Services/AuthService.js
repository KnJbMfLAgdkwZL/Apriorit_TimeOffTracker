import Cookies from 'js-cookie'
import jwt_decode from "jwt-decode"

export class AuthService {
    static isLogged() {
        return Cookies.get("token") !== undefined;
    }

    static logOut() {
        try {
            Cookies.remove('token');
        } catch (e) {
            console.error(e);
        }
    }
}