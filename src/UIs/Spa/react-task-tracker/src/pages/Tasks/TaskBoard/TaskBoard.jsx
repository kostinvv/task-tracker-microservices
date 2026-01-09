import { closestCorners, DndContext, PointerSensor, useSensor, useSensors } from "@dnd-kit/core";
import { useMemo, useState } from "react";
import { TaskColumn } from "./TaskColumn";
import { arrayMove } from "@dnd-kit/sortable";
import './TaskBoard.css';
import { Row } from 'antd';
import { debounce } from "lodash";
import { DND_COLUMN_TYPE, DND_TASK_TYPE } from '../../../constants.js';

export default function TaskBoard() {
    const [columns, setColumns] = useState([
        { 
            id: 'ToDo', 
            title: "To Do",
            tasks: [
                { id: '1', title: 'Item #1' },
                { id: '2', title: 'Item #2' },
                { id: '3', title: 'Item #3' },
                { id: '4', title: 'Item #4' },
                { id: '8', title: 'Item #8' },
                { id: '9', title: 'Item #9' },
                { id: '10', title: 'Item #10' },
            ]
        },
        {
            id: 'InProgress', 
            title: "In Progress",
            tasks: [
                { id: '5', title: 'Item #5' },
                { id: '6', title: 'Item #6' },
            ]
        },
        {
            id: 'Done', 
            title: "Done",
            tasks: [
                { id: '7', title: 'Item #7' },
            ]
        }
    ]);

    const [activeId, setActiveId] = useState(null);

    const sensors = useSensors(
        useSensor(PointerSensor)
    );

    function findValueOfItems(id, dndType) {
        if (dndType === DND_COLUMN_TYPE) {
            return columns.find((column) => column.id === id);
        }

        if (dndType === DND_TASK_TYPE) {
            return columns.find((column) => 
                column.tasks.find((task) => task.id === id)
            );   
        }
    } 

    function handleDragStart(event) {
        const { active } = event;
        const { id } = active;
        setActiveId(id);
    }

    function handleDragOver(event) {
        const { active, over } = event;
        // Handle task sorting.
        if (active.data.current.type === DND_TASK_TYPE &&
            over.data.current.type === DND_TASK_TYPE &&
            active && 
            over && 
            active.id !== over.id
        ) {
            const activeColumn = findValueOfItems(active.id, DND_TASK_TYPE);
            const overColumn = findValueOfItems(over.id, DND_TASK_TYPE);

            if (!activeColumn || !overColumn) {
                return;
            }

            const activeColumnIndex = columns.findIndex((column) => column.id === activeColumn.id);
            const overColumnIndex = columns.findIndex((column) => column.id === overColumn.id);
            const activeTaskIndex = activeColumn.tasks.findIndex((task) => task.id === active.id);
            const overTaskIndex = overColumn.tasks.findIndex((task) => task.id === over.id);

            if (activeColumnIndex === overColumnIndex) {
                let newItems = [...columns];
                newItems[activeColumnIndex].tasks = arrayMove(
                    newItems[activeColumnIndex].tasks,
                    activeTaskIndex,
                    overTaskIndex
                );
                setColumns(newItems);
            } else {
                let newItems = [...columns];
                const [removedItem] = newItems[activeColumnIndex].tasks.splice(
                    activeTaskIndex,
                    1
                );
                newItems[overColumnIndex].tasks.splice(
                    overTaskIndex,
                    0,
                    removedItem
                );
                setColumns(newItems);
            }
        }
        // Handling task drop into a column.
        if (active.data.current.type === DND_TASK_TYPE && 
            over?.data.current.type === DND_COLUMN_TYPE &&
            active && 
            over && 
            active.id !== over.id
        ) {
            const activeColumn = findValueOfItems(active.id, DND_TASK_TYPE);
            const overColumn = findValueOfItems(over.id, DND_COLUMN_TYPE);

            if (!activeColumn || !overColumn) {
                return;
            }

            const activeColumnIndex = columns.findIndex((column) => column.id === activeColumn.id);
            const overColumnIndex = columns.findIndex((column) => column.id === overColumn.id);
            const activeTaskIndex = activeColumn.tasks.findIndex((task) => task.id === active.id);

            let newItems = [...columns];
            const [removedItem] = newItems[activeColumnIndex].tasks.splice(
                activeTaskIndex,
                1
            );
            newItems[overColumnIndex].tasks.push(removedItem);
            setColumns(newItems);
        }
    }

    const handleDragOverDebounced = useMemo(
        () => debounce(handleDragOver, 25), 
        [setColumns]
    );

    function handleDragEnd(event) {
        setActiveId(null);
    }

    return (
        <DndContext
            sensors={sensors}
            collisionDetection={closestCorners}
            onDragStart={handleDragStart}
            onDragOver={handleDragOverDebounced}
            onDragEnd={handleDragEnd}
        >
            <Row gutter={[16, 16]}>
                {columns.map((column) =>
                    <TaskColumn key={column.id} id={column.id} title={column.title} tasks={column.tasks} />
                )}                
            </Row>
        </DndContext>
    )
}