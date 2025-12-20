import { useSortable } from "@dnd-kit/sortable"
import { CSS } from "@dnd-kit/utilities";

export function TaskItem({id, title}) {
    const { attributes, listeners, setNodeRef, transform, transition, isDragging } = useSortable({
        id: id
    });

    const style = {
        transform: CSS.Transform.toString(transform),
        transition,
        opacity: isDragging ? 0.5 : 1
    };

    return (
        <div 
            ref={setNodeRef}
            style={style}
            {...attributes}
            {...listeners}
            className='board-item'
        >
            <div className="board-item-content">{ title }</div>
        </div>
    )
}