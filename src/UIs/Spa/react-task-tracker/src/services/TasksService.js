import api from '../http';

export default class TasksService {
    static async getTasks() {
        return await api.get('/tasks');
    }
}