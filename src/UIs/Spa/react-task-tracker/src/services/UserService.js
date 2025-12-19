import api from '../http';

export default class UserService {
    static async logIn(email, password) {
        return await api.post('/user/login', {
            email,
            password
        });
    }

    static async signUp(email, password) {
        return await api.post('/user', {
            email,
            password
        });
    }

    static async getUser() {
        return await api.get('/user');
    }
}
