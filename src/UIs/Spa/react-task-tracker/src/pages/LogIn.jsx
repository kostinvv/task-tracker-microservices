import { useContext, useState } from "react"
import { Context } from '../main.jsx';
import { observer } from "mobx-react-lite";
import { Button, Form, Input, Alert } from "antd";
import { useNavigate } from "react-router-dom";

function LogIn() {
    const { store } = useContext(Context);
    const [errorMessage, setErrorMessage] = useState(null);
    const navigate = useNavigate();

    async function onFinish (values) {
        const { email, password } = values;
        const errorMessage = await store.logIn(email, password);
        
        if (errorMessage) {
            setErrorMessage(errorMessage);
        } else {
            navigate('/tasks');
        }
    }

    function onFinishFailed (errorInfo) {
        setErrorMessage(null);
    }

    return (
        <Form
            name="basic"
            layout='vertical'
            style={{ maxWidth: 600 }}
            onFinish={onFinish}
            onFinishFailed={onFinishFailed}
            autoComplete="off"
        >
            <h1>Log In</h1>
            { errorMessage ? <Alert style={{ marginBottom: 16 }} type="error" title={errorMessage} banner /> : null }
            <Form.Item
                label='E-mail'
                name='email'
                rules={[{ required: true, message: 'Please input your e-mail' }]}
            >
                <Input placeholder="Enter your e-mail" />
            </Form.Item>
            <Form.Item
                label='Password'
                name='password'
                rules={[{ required: true, message: 'Please input your password' }]}
            >
                <Input.Password placeholder="Enter your e-mail" />
            </Form.Item>
            <Form.Item label={null}>
                <Button type="primary" htmlType="submit">
                    Log In
                </Button>
            </Form.Item>
        </Form>
    )
}

export default observer(LogIn);