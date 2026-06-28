import { apiClient } from './client'
import type { User } from '../types'

export const authApi = {
  login: async (email: string, password: string) => {
    const res = await apiClient.post('/auth/login', { email, password })
    return res.data
  },
  register: async (data: { name: string; email: string; password: string }) => {
    const res = await apiClient.post('/auth/register', data)
    return res.data
  },
  me: async (): Promise<User> => {
    const res = await apiClient.get('/auth/me')
    return res.data
  },
  logout: async () => {
    await apiClient.post('/auth/logout')
  },
}
