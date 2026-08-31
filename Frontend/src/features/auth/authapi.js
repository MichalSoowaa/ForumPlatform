import { apiClient } from '../../api/client';

// Shapes match RegisterUserCommand / LoginQuery on the backend exactly

export async function registerUser({ email, username, password, passwordConfirm }) {
    const { data } = await apiClient.post('/auth/register', {
        email,
        username,
        password,
        passwordConfirm,
    });

    return data; // RegisterUserResponse
}

export async function loginUser({ email, password }) {
    const { data } = await apiClient.post('/auth/login', { email, password });
    return data;
}