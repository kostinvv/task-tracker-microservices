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

    async getTasks() {
        try {
            const response = await TasksService.getTasks();
            this.setColumns(response.data);
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
                column.tasks.find((task) => task.id === id)
            );   
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
        const activeTaskIndex = activeColumn.tasks.findIndex((task) => task.id === activeId);

        let newItems = [...this.columns];
        const [removedItem] = newItems[activeColumnIndex].tasks.splice(
            activeTaskIndex,
            1
        );
        newItems[overColumnIndex].tasks.push(removedItem);
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
        const activeTaskIndex = activeColumn.tasks.findIndex((task) => task.id === activeId);
        const overTaskIndex = overColumn.tasks.findIndex((task) => task.id === overId);

        if (activeColumnIndex === overColumnIndex) {
            let newItems = [...this.columns];
            newItems[activeColumnIndex].tasks = arrayMove(
                newItems[activeColumnIndex].tasks,
                activeTaskIndex,
                overTaskIndex
            );
            this.setColumns(newItems);
        } else {
            let newItems = [...this.columns];
            const [removedItem] = newItems[activeColumnIndex].tasks.splice(
                activeTaskIndex,
                1
            );
            newItems[overColumnIndex].tasks.splice(
                overTaskIndex,
                0,
                removedItem
            );
            this.setColumns(newItems);
        }
    }
}