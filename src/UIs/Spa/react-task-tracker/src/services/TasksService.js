import api from '../http';

export default class TasksService {
    static pageSize = 3;

    static async getBoard() {
        return await api.get(`/tasks/board?size=${TasksService.pageSize}`);
    }

    static async getTasks(afterPosition, state) {
        return await api.get(`/tasks?size=${TasksService.pageSize}&afterPosition=${afterPosition}&state=${state}`);
    }

    static async move(taskId, prevOrder, nextOrder, newState) {
        return await api.patch(`/tasks/${taskId}/move`, {
            prevOrder,
            nextOrder,
            newState
        });
    }
}