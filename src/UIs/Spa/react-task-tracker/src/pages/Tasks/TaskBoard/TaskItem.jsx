import { useSortable } from "@dnd-kit/sortable"
import { CSS } from "@dnd-kit/utilities";
import { EditOutlined, EllipsisOutlined, CheckOutlined, SettingOutlined, DeleteOutlined } from '@ant-design/icons';
import { Avatar, Card, Button } from 'antd';
import { DND_TASK_TYPE } from '../../../constants.js';
import { createStyles } from 'antd-style';

const { Meta } = Card;

export function TaskItem({id, title}) {
    const { attributes, listeners, setNodeRef, transform, transition, isDragging } = useSortable({
        id: id,
        data: {
            id: id,
            title: title,
            type: DND_TASK_TYPE
        }
    });

    const style = {
        transform: CSS.Transform.toString(transform),
        transition,
        opacity: isDragging ? 0 : 1
    };

    const actions = [
        <CheckOutlined style={{ color: '#52c41a' }} />,
        <SettingOutlined />,
        <DeleteOutlined style={{ color: '#eb2f58' }} />
    ];

    return (
        <div 
            ref={setNodeRef}
            style={style}
            {...attributes}
            {...listeners}
        >
            <Card
                hoverable
                size="small"
                style={{ marginBottom: 12 }}
                actions={actions}
            >
                <Meta title={title} description="10:03 1/13/2026" />
            </Card>
        </div>
    )
}