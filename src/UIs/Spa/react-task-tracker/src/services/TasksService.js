import api from '../http';

export default class TasksService {
    static async getBoard() {
        return await api.get('/tasks/board?size=10');
    }

    static async move(taskId, prevOrder, nextOrder, newState) {
        return await api.patch(`/tasks/${taskId}/move`, {
            prevOrder,
            nextOrder,
            newState
        });
    }
}