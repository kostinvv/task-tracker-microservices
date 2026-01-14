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
            const response = await TasksService.getBoard();
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

    saveTaskMove = (prevActiveIndex, activeId) => {
        const activeColumn = this.findValueOfItems(activeId, DND_TASK_TYPE);

        if (!activeColumn) {
            return;
        }

        const newActiveTaskIndex = activeColumn.pagedList.items.findIndex((task) => task.id === activeId);
        const activeColumnIndex = this.columns.findIndex((column) => column.id === activeColumn.id);

        try {
            TasksService.move(
                activeId, 
                prevActiveIndex, 
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
}