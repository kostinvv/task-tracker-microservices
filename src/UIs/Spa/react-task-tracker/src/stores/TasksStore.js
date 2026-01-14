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
        return this.columns[id].cursorList.items.length - 1
    }

    async getBoard() {
        try {
            const response = await TasksService.getBoard();
            this.setColumns(response.data);
        } catch (error) {
            console.log(error.response?.data?.title);
        }
    }

    loadMoreTasks = async (afterPosition, state) => {
        try {
            const response = await TasksService.getTasks(afterPosition, state);
            const columnIndex = this.columns.findIndex((column) => column.id === state);

            const currentColumn = this.columns[columnIndex];
            const existingIds = new Set(currentColumn.cursorList.items.map(task => task.id));
            const uniqueIds = response.data.items.filter(task => !existingIds.has(task.id));

            currentColumn.cursorList.items.push(...uniqueIds);
            currentColumn.cursorList.hasNextPage = response.data.hasNextPage;
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
                column.cursorList.items.find((task) => task.id === id)
            );   
        }
    }
    
    getTaskIndex = (id) => {
        const column = this.findValueOfItems(id, DND_TASK_TYPE);
        if (!column) {
            return;
        }
        return column.cursorList.items.findIndex((task) => task.id === id);
    }

    saveTaskMove = (prevActiveIndex, activeId) => {
        const activeColumn = this.findValueOfItems(activeId, DND_TASK_TYPE);

        if (!activeColumn) {
            return;
        }

        const newActiveTaskIndex = activeColumn.cursorList.items.findIndex((task) => task.id === activeId);
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

        const activeTaskIndex = activeColumn.cursorList.items.findIndex((task) => task.id === activeId);

        let newItems = [...this.columns];
        const [removedItem] = newItems[activeColumnIndex].cursorList.items.splice(
            activeTaskIndex,
            1
        );
        newItems[overColumnIndex].cursorList.items.push(removedItem);
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
        const activeTaskIndex = activeColumn.cursorList.items.findIndex((task) => task.id === activeId);
        const overTaskIndex = overColumn.cursorList.items.findIndex((task) => task.id === overId);
        
        if (activeColumnIndex === overColumnIndex) {
            let newItems = [...this.columns];
            newItems[activeColumnIndex].cursorList.items = arrayMove(
                newItems[activeColumnIndex].cursorList.items,
                activeTaskIndex,
                overTaskIndex
            );
            this.setColumns(newItems);
        } else {
            let newItems = [...this.columns];
            const [removedItem] = newItems[activeColumnIndex].cursorList.items.splice(
                activeTaskIndex,
                1
            );
            newItems[overColumnIndex].cursorList.items.splice(
                overTaskIndex,
                0,
                removedItem
            );
            this.setColumns(newItems);
        }
    }
}