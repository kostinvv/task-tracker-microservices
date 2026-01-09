import { closestCorners, DndContext, PointerSensor, useSensor, useSensors } from "@dnd-kit/core";
import { useMemo, useContext, useEffect, useState } from 'react';
import { Context } from '../../../main.jsx';
import { observer } from 'mobx-react-lite';
import { TaskColumn } from "./TaskColumn";
import './TaskBoard.css';
import { Row } from 'antd';
import { debounce } from "lodash";
import { DND_COLUMN_TYPE, DND_TASK_TYPE } from '../../../constants.js';

function TaskBoard() {
    const { store } = useContext(Context);

    useEffect(() => {
        store.tasks.getTasks();
    }, []);

    const [activeId, setActiveId] = useState(null);

    const sensors = useSensors(
        useSensor(PointerSensor)
    );

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
            store.tasks.reorderTask(active.id, over.id);
        }
        // Handling task drop into a column.
        if (active.data.current.type === DND_TASK_TYPE && 
            over?.data.current.type === DND_COLUMN_TYPE &&
            active && 
            over && 
            active.id !== over.id
        ) {
            store.tasks.moveTask(active.id, over.id);
        }
    }

    const handleDragOverDebounced = useMemo(
        () => debounce(handleDragOver, 25), 
        [store.tasks.setColumns]
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
                {store.tasks.columns.map((column) =>
                    <TaskColumn key={column.id} id={column.id} title={column.title} tasks={column.tasks} />
                )}                
            </Row>
        </DndContext>
    )
}

export default observer(TaskBoard);
