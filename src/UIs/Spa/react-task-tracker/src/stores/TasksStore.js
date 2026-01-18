import { makeAutoObservable } from "mobx";
import TasksService from "../services/TasksService";
import { DND_COLUMN_TYPE, DND_TASK_TYPE } from "../constants";
import { arrayMove } from "@dnd-kit/sortable";

export default class TasksStore {
    columns = [];

    constructor() {
        makeAutoObservable(this);
    }

    setColumns = (columns) => {
        this.columns = columns;
    }

    getLastPosition = (id) => {
        return this.columns[id].pagedList.items.length;
    }

    async getBoard() {
        try {
            const response = await TasksService.getBoard();
            this.setColumns(response.data);
        } catch (error) {
            console.log(error.response?.data?.title);
        }
    }

    loadMoreTasks = async (skip, state) => {
        try {
            const response = await TasksService.getTasks(skip, state);
            const columnIndex = this.columns.findIndex((column) => column.id === state);

            const currentColumn = this.columns[columnIndex];
            const existingIds = new Set(currentColumn.pagedList.items.map(task => task.id));
            const uniqueIds = response.data.items.filter(task => !existingIds.has(task.id));

            currentColumn.pagedList.items.push(...uniqueIds);
            currentColumn.pagedList.hasNextPage = response.data.hasNextPage;
            this.setColumns([...this.columns]);

        } catch (error) {
            console.log(error.response?.data?.title);            
        }
    }

    findValueOfItems = (id, dndType) => {
        if (dndType === DND_COLUMN_TYPE) {
            return this.columns.find((column) => column.id === id);
        }

        if (dndType === DND_TASK_TYPE) {
            return this.columns.find((column) => 
                column.pagedList.items.find((task) => task.id === id)
            );   
        }
    }
    
    getTaskIndex = (id) => {
        const column = this.findValueOfItems(id, DND_TASK_TYPE);
        if (!column) {
            return;
        }
        return column.pagedList.items.findIndex((task) => task.id === id);
    }

    saveTaskMove = (activeId) => {
        const activeColumn = this.findValueOfItems(activeId, DND_TASK_TYPE);

        if (!activeColumn) {
            return;
        }

        const newActiveTaskIndex = activeColumn.pagedList.items.findIndex((task) => task.id === activeId);
        const activeColumnIndex = this.columns.findIndex((column) => column.id === activeColumn.id);

        try {
            TasksService.move(
                activeId, 
                newActiveTaskIndex,
                activeColumnIndex
            );            
        } catch (error) {
             console.log(error.response?.data?.title);
        }
    }

    moveTask = (activeId, overId) => {
        const activeColumn = this.findValueOfItems(activeId, DND_TASK_TYPE);
        const overColumn = this.findValueOfItems(overId, DND_COLUMN_TYPE);
        if (!activeColumn || !overColumn) {
            return;
        }

        const activeColumnIndex = this.columns.findIndex((column) => column.id === activeColumn.id);
        const overColumnIndex = this.columns.findIndex((column) => column.id === overColumn.id);

        if (activeColumnIndex === overColumnIndex) {
            return;
        }

        const activeTaskIndex = activeColumn.pagedList.items.findIndex((task) => task.id === activeId);

        let newItems = [...this.columns];
        const [removedItem] = newItems[activeColumnIndex].pagedList.items.splice(
            activeTaskIndex,
            1
        );
        newItems[overColumnIndex].pagedList.items.push(removedItem);
        this.setColumns(newItems);
    }

    reorderTask = (activeId, overId) => {
        const activeColumn = this.findValueOfItems(activeId, DND_TASK_TYPE);
        const overColumn = this.findValueOfItems(overId, DND_TASK_TYPE);

        if (!activeColumn || !overColumn) {
            return;
        }
        
        const activeColumnIndex = this.columns.findIndex((column) => column.id === activeColumn.id);
        const overColumnIndex = this.columns.findIndex((column) => column.id === overColumn.id);
        const activeTaskIndex = activeColumn.pagedList.items.findIndex((task) => task.id === activeId);
        const overTaskIndex = overColumn.pagedList.items.findIndex((task) => task.id === overId);
        
        if (activeColumnIndex === overColumnIndex) {
            let newItems = [...this.columns];
            newItems[activeColumnIndex].pagedList.items = arrayMove(
                newItems[activeColumnIndex].pagedList.items,
                activeTaskIndex,
                overTaskIndex
            );
            this.setColumns(newItems);
        } else {
            let newItems = [...this.columns];
            const [removedItem] = newItems[activeColumnIndex].pagedList.items.splice(
                activeTaskIndex,
                1
            );
            newItems[overColumnIndex].pagedList.items.splice(
                overTaskIndex,
                0,
                removedItem
            );
            this.setColumns(newItems);
        }
    }

    getTask = async (id) => {
        try {
            const response = await TasksService.getTask(id);
            return response.data;
        } catch (error) {
            console.error(error.response?.data?.title);
        }
    }

    createTask = async (request) => {
        try {
            let sortOrder = this.getLastPosition(request.state)

            const response = await TasksService.createTask({
                ...request,
                sortOrder
            });

            const newItem = response.data;
            this.columns[request.state].pagedList.items.splice(sortOrder, 0, newItem);
            this.setColumns([...this.columns]);

            return response;
        } catch (error) {
            console.error(error.response?.data?.title);
        }
    }

    deleteTask = async (id, state) => {
        try {
            const response = await TasksService.deleteTask(id);

            let newItems = [...this.columns];
            const column = newItems[state];
            const activeTaskIndex = column.pagedList.items.findIndex((task) => task.id === id);

            column.pagedList.items.splice(activeTaskIndex, 1);
            this.setColumns(newItems);

            return response;
        } catch (error) {
            console.error(error.response?.data?.title);
        }
    }

    updateTask = async (taskId, taskState, updateTaskRequest) => {
        // доделать завтра (на backend нужно тоже другой request передавать).
        try {
            await TasksService.updateTask(taskId, {
                title: updateTaskRequest.title,
                description: updateTaskRequest.description
            });

            let newItems = [...this.columns];
            const column = newItems[taskState];

            const activeTaskIndex = column.pagedList.items.findIndex((task) => task.id === taskId);

            const task = column.pagedList.items[activeTaskIndex];
            
            task.title = updateTaskRequest.title;
            task.description = updateTaskRequest.description;

            if (taskState !== updateTaskRequest.state) {
                const newOrder = 0;

                await TasksService.move(taskId, newOrder, updateTaskRequest.state);

                const [removedItem] = newItems[taskState].pagedList.items.splice(
                    activeTaskIndex,
                    1
                );

                newItems[updateTaskRequest.state].pagedList.items.splice(newOrder, 0, removedItem);
                task.sortOrder = newOrder;
                task.state = updateTaskRequest.state;
            }

            this.setColumns(newItems);
        } catch (error) {
            console.error(error.response?.data?.title);
        }
    }
}