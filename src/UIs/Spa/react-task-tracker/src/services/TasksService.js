import api from '../http';

export default class TasksService {
    static pageSize = 3;

    static async getBoard() {
        return await api.get(`/tasks/board?size=${TasksService.pageSize}`);
    }

    static async getTasks(skip, state) {
        return await api.get(`/tasks?size=${TasksService.pageSize}&skip=${skip}&state=${state}`);
    }

    static async getTask(id) {
        return await api.get(`/tasks/${id}`);
    }

    static async move(taskId, newOrder, newState) {
        return await api.patch(`/tasks/${taskId}/move`, {
            newOrder,
            newState
        });
    }

    static async createTask(request) {
        return await api.post('/tasks', request);
    }

    static async deleteTask(id) {
        return await api.delete(`/tasks/${id}`);
    }

    static async updateTask(id, request) {
        return await api.put(`/tasks/${id}`, request);
    }
}