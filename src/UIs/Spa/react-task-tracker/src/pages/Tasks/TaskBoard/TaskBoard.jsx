import { closestCorners, DndContext, PointerSensor, useSensor, useSensors } from "@dnd-kit/core";
import { useState } from "react";
import { TaskColumn } from "./TaskColumn";
import { arrayMove } from "@dnd-kit/sortable";
import './TaskBoard.css';
import { Row } from 'antd';

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

    const sensors = useSensors(
        useSensor(PointerSensor)
    );

    return (
        <DndContext
            sensors={sensors}
            collisionDetection={closestCorners}
        >
            <Row gutter={[16, 16]}>
                {columns.map((column) =>
                    <TaskColumn key={column.id} id={column.id} title={column.title} tasks={column.tasks} />
                )}                
            </Row>
        </DndContext>
    )
}