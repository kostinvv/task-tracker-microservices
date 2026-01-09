import { makeAutoObservable } from "mobx";
import UserService from '../services/UserService.js';
import axios from "axios";
import api, { API_URL } from "../http/index.js";

export default class Store {
    user = {};
    isAuth = false;

    constructor() {
        makeAutoObservable(this);
    }

    setAuth(isAuth) {
        this.isAuth = isAuth;
    }

    setUser(user) {
        this.user = user;
    }

    async checkAuth() {
        try {
            const response = await UserService.getUser();
            this.setUser(response.data);
            this.setAuth(true);
        } catch (error) {
            console.error(error.response?.data?.title);
        }
    }

    async logIn(email, password) {
        try {
            const response = await UserService.logIn(email, password);
            localStorage.setItem('accessToken', response.data.accessToken);
            await this.checkAuth();
        } catch (error) {
            const errorMessage = error.response?.data?.title;
            console.error(errorMessage);
            return errorMessage;
        }
    }

    async signUp(email, password) {
        try {
            const response = await UserService.signUp(email, password);
            localStorage.setItem('accessToken', response.data.accessToken);
            await this.checkAuth();
        } catch (error) {
            const errorMessage = error.response?.data?.title;
            console.error(errorMessage);
            return errorMessage;
        }
    }

    logOut() {
        localStorage.removeItem('accessToken');
        this.setAuth(false);
        this.setUser({});
    }
}
