import { apiClient } from './client'

export const vaultApi = {
  getSecrets: async () => {
    const res = await apiClient.get('/vault/secrets')
    return res.data
  },
  createSecret: async (data: { name: string; value: string; path: string }) => {
    const res = await apiClient.post('/vault/secrets', data)
    return res.data
  },
  rotateSecret: async (id: string) => {
    const res = await apiClient.post(`/vault/secrets/${id}/rotate`)
    return res.data
  },
  deleteSecret: async (id: string) => {
    await apiClient.delete(`/vault/secrets/${id}`)
  },
}
