import AuthStore from './AuthStore';
import TasksStore from './TasksStore';

export default class RootStore {
    auth = new AuthStore();
    tasks = new TasksStore();
}