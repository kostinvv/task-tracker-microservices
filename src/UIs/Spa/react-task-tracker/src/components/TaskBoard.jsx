import { closestCorners, DndContext, PointerSensor, useSensor, useSensors } from "@dnd-kit/core";
import { useState } from "react";
import { TaskColumn } from "./TaskColumn";
import { arrayMove } from "@dnd-kit/sortable";
import './Board.css';

export function TaskBoard() {
    const [columns, setColumns] = useState([
        { 
            id: 'ToDo', 
            title: "To Do",
            tasks: [
                { id: '1', title: 'Item #1' },
                { id: '2', title: 'Item #2' },
                { id: '3', title: 'Item #3' },
                { id: '4', title: 'Item #4' },
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

    function findColumn(id) {
        if (!id) return null;

        if (columns.some((c) => c.id === id)) {
            return columns.find((c) => c.id === id) ?? null;
        }
        
        const itemWithColumnId = columns.flatMap((c) => {
            return c.tasks.map((task) => ({ 
                itemId: task.id, 
                columnId: c.id 
            }));
        });
        
        const columnId = itemWithColumnId.find((i) => i.itemId === id)?.columnId;
        return columns.find((c) => c.id === columnId) ?? null;
    }

    function handleDragEnd(event) {
        const { active, over } = event;
        const activeId = active.id;
        const overId = over ? over.id : null;

        const activeColumn = findColumn(activeId);
        const overColumn = findColumn(overId);

        if (!activeColumn || !overColumn || activeColumn !== overColumn) return null;
        
        const activeIndex = activeColumn.tasks.findIndex((i) => i.id === activeId);
        const overIndex = overColumn.tasks.findIndex((i) => i.id === overId);

        if (activeIndex !== overIndex) {
            setColumns((prevState) => {
                return prevState.map((column) => {
                    if (column.id === activeColumn.id) {
                        column.tasks = arrayMove(overColumn.tasks, activeIndex, overIndex);
                        return column;
                    } else {
                        return column;
                    }
                });
            });
        }
    }

    function handleDragOver(event) {
        const { active, over, delta } = event;

        const activeId = active.id;
        const overId = over ? over.id : null;

        const activeColumn = findColumn(activeId);
        const overColumn = findColumn(overId);
        
        if (!activeColumn || !overColumn || activeColumn === overColumn) return null;

        setColumns((prevState) => {
            const activeItems = activeColumn.tasks;
            const overItems = overColumn.tasks;

            const activeIndex = activeItems.findIndex((i) => i.id === activeId);
            const overIndex = overItems.findIndex((i) => i.id === overId);

            const newIndex = () => {
                const putOnBelowLastItem = overIndex === overItems.length - 1 && delta.y > 0;
                const modifier = putOnBelowLastItem ? 1 : 0;
                return overIndex >= 0 ? overIndex + modifier : overItems.length + 1;
            };

            return prevState.map((c) => {
                if (c.id === activeColumn.id) {
                    c.tasks = activeItems.filter((i) => i.id !== activeId);
                    return c;
                } else if (c.id === overColumn.id) {
                    c.tasks = [
                        ...overItems.slice(0, newIndex()),
                        activeItems[activeIndex],
                        ...overItems.slice(newIndex(), overItems.length)
                    ];
                    return c;
                } else {
                    return c;
                }
            });
        });
    }

    const sensors = useSensors(
        useSensor(PointerSensor)
    );

    return (
        <DndContext
            sensors={sensors}
            collisionDetection={closestCorners}
            onDragEnd={handleDragEnd}
            onDragOver={handleDragOver}
        >
            <div className="board">
                {columns.map((column) =>
                    <TaskColumn key={column.id} id={column.id} title={column.title} tasks={column.tasks} />
                )}
            </div>
        </DndContext>
    )
}