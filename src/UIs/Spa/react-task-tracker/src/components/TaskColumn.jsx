import { useDroppable } from "@dnd-kit/core";
import { rectSortingStrategy, SortableContext } from "@dnd-kit/sortable";
import { TaskItem } from "./TaskItem";

export function TaskColumn({ id, title, tasks }) {
    const { setNodeRef } = useDroppable({ id: id });
    return (
        <SortableContext id={id} items={tasks.map((task => task.id))} strategy={rectSortingStrategy}>
            <div ref={setNodeRef} className='column'>
                <div className='board-column-header'>{title}</div>
                <div className='board-column-content-wrapper'>
                    {tasks.map((task) => 
                        <TaskItem key={task.id} id={task.id} title={task.title} />
                    )}
                </div>
            </div>
        </SortableContext>
    )
}