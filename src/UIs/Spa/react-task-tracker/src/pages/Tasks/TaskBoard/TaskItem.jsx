import { useSortable } from "@dnd-kit/sortable"
import { CSS } from "@dnd-kit/utilities";
import { EditOutlined, EllipsisOutlined } from '@ant-design/icons';
import { Avatar, Card, Button } from 'antd';

const { Meta } = Card;

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
        >
            <Card
                size="small"
                style={{ marginBottom: 12 }}
                title={title}
            >
                Content
            </Card>
        </div>
    )
}