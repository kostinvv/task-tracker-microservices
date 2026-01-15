import api from '../http';

export default class TasksService {
    static pageSize = 3;

    static async getBoard() {
        return await api.get(`/tasks/board?size=${TasksService.pageSize}`);
    }

    static async getTasks(afterPosition, state) {
        return await api.get(`/tasks?size=${TasksService.pageSize}&afterPosition=${afterPosition}&state=${state}`);
    }

    static async getTask(id) {
        return await api.get(`/tasks/${id}`);
    }

    static async move(taskId, prevOrder, nextOrder, newState) {
        return await api.patch(`/tasks/${taskId}/move`, {
            prevOrder,
            nextOrder,
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