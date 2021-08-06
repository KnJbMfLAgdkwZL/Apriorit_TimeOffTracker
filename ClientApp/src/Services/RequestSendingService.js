export class RequestSendingService {
    static async sendPostRequestUnauthorized(url, body) {
        const requestOptions = {
            method: 'POST',
            headers: {'Content-Type': 'application/json'},
            body: JSON.stringify(body)
        };
        return await fetch(url, requestOptions)
            .catch(error => console.error(error));
    }

    static async sendPostRequestAuthorized(url, body) {
        const requestOptions = {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + localStorage.getItem("token")
            },
            body: JSON.stringify(body)
        };
        return await fetch(url, requestOptions)
            .catch(error => console.error(error));
    }

    static async sendPatchRequestUnauthorized(url, body) {
        const requestOptions = {
            method: 'PATCH',
            headers: {'Content-Type': 'application/json'},
            body: JSON.stringify(body)
        };
        return await fetch(url, requestOptions)
            .catch(error => console.error(error));
    }

    static async sendPatchRequestAuthorized(url, body) {
        const requestOptions = {
            method: 'PATCH',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + localStorage.getItem("token")
            },
            body: JSON.stringify(body)
        };
        return await fetch(url, requestOptions)
            .catch(error => console.error(error));
    }

    static async sendGetRequestUnauthorized(url) {
        return await fetch(url)
            .catch(error => console.error(error));
    }

    static async sendGetRequestAuthorized(url) {
        const requestOptions = {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + localStorage.getItem("token")
            }
        };
        return await fetch(url, requestOptions)
            .catch(error => console.error(error));
    }
}