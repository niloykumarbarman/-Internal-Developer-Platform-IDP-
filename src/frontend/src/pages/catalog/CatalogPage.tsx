import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { apiClient } from '../../api/client'
import { Plus, Search, BookOpen, Loader2, X } from 'lucide-react'

const StatusBadge = ({ status }: { status: string }) => {
  const map: Record<string, string> = {
    Active: 'oklch(0.6 0.18 150)', Deprecated: 'oklch(0.6 0.22 27)', Experimental: 'oklch(0.75 0.18 80)'
  }
  const c = map[status] || 'oklch(0.55 0 0)'
  return <span style={{ fontSize: 11, fontWeight: 600, padding: '2px 8px', borderRadius: 20, color: c, background: `${c.replace(')', ' / 0.12)')}` }}>{status}</span>
}

const mockData = [
  { id: '1', name: 'api-gateway', type: 'Service', status: 'Active', owner: 'Platform Team', language: 'Go', description: 'Main API gateway for all microservices' },
  { id: '2', name: 'auth-service', type: 'Service', status: 'Active', owner: 'Security Team', language: 'C#', description: 'JWT + OAuth2 authentication service' },
  { id: '3', name: 'user-service', type: 'Service', status: 'Active', owner: 'Core Team', language: 'C#', description: 'User management and profiles' },
  { id: '4', name: 'notification-svc', type: 'Service', status: 'Experimental', owner: 'Core Team', language: 'Node.js', description: 'Email and push notification service' },
  { id: '5', name: 'legacy-billing', type: 'Service', status: 'Deprecated', owner: 'Finance Team', language: 'Java', description: 'Old billing system (being replaced)' },
]

export default function CatalogPage() {
  const [search, setSearch] = useState('')
  const [showForm, setShowForm] = useState(false)
  const [form, setForm] = useState({ name: '', type: 'Service', description: '', owner: '', language: '' })
  const qc = useQueryClient()

  const { data, isLoading } = useQuery({
    queryKey: ['catalog'],
    queryFn: () => apiClient.get('/services').then(r => r.data),
    retry: false,
  })

  const createMutation = useMutation({
    mutationFn: (payload: any) => apiClient.post('/services', payload),
    onSuccess: () => { qc.invalidateQueries({ queryKey: ['catalog'] }); setShowForm(false); setForm({ name: '', type: 'Service', description: '', owner: '', language: '' }) },
  })

  const services = data?.items ?? mockData
  const filtered = services.filter((s: any) =>
    s.name?.toLowerCase().includes(search.toLowerCase()) ||
    s.owner?.toLowerCase().includes(search.toLowerCase())
  )

  const inp: React.CSSProperties = { width: '100%', padding: '0.55rem 0.75rem', background: 'oklch(0.22 0 0)', border: '1px solid oklch(0.32 0 0)', borderRadius: 6, color: 'oklch(0.95 0 0)', fontSize: 13, outline: 'none', boxSizing: 'border-box' }

  return (
    <div style={{ display: 'flex', flexDirection: 'column', gap: '1.25rem' }}>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start' }}>
        <div>
          <h1 style={{ fontSize: 22, fontWeight: 700, color: 'oklch(0.95 0 0)', marginBottom: 4 }}>Service Catalog</h1>
          <p style={{ fontSize: 13, color: 'oklch(0.5 0 0)' }}>{filtered.length} services registered</p>
        </div>
        <button onClick={() => setShowForm(true)} style={{ display: 'flex', alignItems: 'center', gap: 6, padding: '0.55rem 1rem', background: 'oklch(0.6 0.2 250)', border: 'none', borderRadius: 6, color: '#fff', fontSize: 13, fontWeight: 600, cursor: 'pointer' }}>
          <Plus size={15} /> Register Service
        </button>
      </div>

      {/* Search */}
      <div style={{ position: 'relative' }}>
        <Search size={15} style={{ position: 'absolute', left: 12, top: '50%', transform: 'translateY(-50%)', color: 'oklch(0.5 0 0)' }} />
        <input value={search} onChange={e => setSearch(e.target.value)} placeholder="Search services..." style={{ ...inp, paddingLeft: 36, width: '100%' }} />
      </div>

      {/* Register Form Modal */}
      {showForm && (
        <div style={{ position: 'fixed', inset: 0, background: 'rgba(0,0,0,0.7)', display: 'flex', alignItems: 'center', justifyContent: 'center', zIndex: 50 }}>
          <div style={{ background: 'oklch(0.18 0 0)', border: '1px solid oklch(0.3 0 0)', borderRadius: 12, padding: '1.5rem', width: 420 }}>
            <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: '1.25rem' }}>
              <h2 style={{ fontSize: 16, fontWeight: 700, color: 'oklch(0.95 0 0)' }}>Register Service</h2>
              <button onClick={() => setShowForm(false)} style={{ background: 'none', border: 'none', cursor: 'pointer', color: 'oklch(0.5 0 0)' }}><X size={18} /></button>
            </div>
            <div style={{ display: 'flex', flexDirection: 'column', gap: '0.85rem' }}>
              {[['name', 'Service Name'], ['owner', 'Owner Team'], ['language', 'Language/Tech']].map(([field, label]) => (
                <div key={field}>
                  <label style={{ display: 'block', fontSize: 12, color: 'oklch(0.6 0 0)', marginBottom: 4 }}>{label}</label>
                  <input style={inp} value={(form as any)[field]} onChange={e => setForm({ ...form, [field]: e.target.value })} placeholder={label} />
                </div>
              ))}
              <div>
                <label style={{ display: 'block', fontSize: 12, color: 'oklch(0.6 0 0)', marginBottom: 4 }}>Type</label>
                <select style={{ ...inp, cursor: 'pointer' }} value={form.type} onChange={e => setForm({ ...form, type: e.target.value })}>
                  {['Service', 'Library', 'Website', 'API', 'Database'].map(t => <option key={t}>{t}</option>)}
                </select>
              </div>
              <div>
                <label style={{ display: 'block', fontSize: 12, color: 'oklch(0.6 0 0)', marginBottom: 4 }}>Description</label>
                <textarea style={{ ...inp, height: 70, resize: 'none' }} value={form.description} onChange={e => setForm({ ...form, description: e.target.value })} placeholder="Short description..." />
              </div>
              <button onClick={() => createMutation.mutate(form)} disabled={createMutation.isPending} style={{ padding: '0.65rem', background: 'oklch(0.6 0.2 250)', border: 'none', borderRadius: 6, color: '#fff', fontSize: 13, fontWeight: 600, cursor: 'pointer', display: 'flex', alignItems: 'center', justifyContent: 'center', gap: 6 }}>
                {createMutation.isPending ? <><Loader2 size={14} /> Registering...</> : 'Register Service'}
              </button>
            </div>
          </div>
        </div>
      )}

      {/* Table */}
      <div style={{ background: 'oklch(0.18 0 0)', border: '1px solid oklch(0.28 0 0)', borderRadius: 10, overflow: 'hidden' }}>
        <table style={{ width: '100%', borderCollapse: 'collapse' }}>
          <thead>
            <tr style={{ borderBottom: '1px solid oklch(0.25 0 0)' }}>
              {['Service', 'Type', 'Owner', 'Language', 'Status'].map(h => (
                <th key={h} style={{ padding: '0.75rem 1rem', textAlign: 'left', fontSize: 12, fontWeight: 600, color: 'oklch(0.5 0 0)', textTransform: 'uppercase', letterSpacing: '0.05em' }}>{h}</th>
              ))}
            </tr>
          </thead>
          <tbody>
            {isLoading ? (
              <tr><td colSpan={5} style={{ textAlign: 'center', padding: '2rem', color: 'oklch(0.5 0 0)' }}><Loader2 size={20} /></td></tr>
            ) : filtered.map((svc: any) => (
              <tr key={svc.id} style={{ borderBottom: '1px solid oklch(0.22 0 0)' }}>
                <td style={{ padding: '0.85rem 1rem' }}>
                  <div style={{ display: 'flex', alignItems: 'center', gap: 8 }}>
                    <div style={{ width: 28, height: 28, borderRadius: 6, background: 'oklch(0.6 0.2 250 / 0.15)', display: 'flex', alignItems: 'center', justifyContent: 'center' }}><BookOpen size={13} color="oklch(0.6 0.2 250)" /></div>
                    <div>
                      <div style={{ fontSize: 13, fontWeight: 600, color: 'oklch(0.9 0 0)' }}>{svc.name}</div>
                      <div style={{ fontSize: 11, color: 'oklch(0.5 0 0)' }}>{svc.description}</div>
                    </div>
                  </div>
                </td>
                <td style={{ padding: '0.85rem 1rem', fontSize: 13, color: 'oklch(0.65 0 0)' }}>{svc.type}</td>
                <td style={{ padding: '0.85rem 1rem', fontSize: 13, color: 'oklch(0.65 0 0)' }}>{svc.owner}</td>
                <td style={{ padding: '0.85rem 1rem', fontSize: 13, color: 'oklch(0.65 0 0)' }}>{svc.language}</td>
                <td style={{ padding: '0.85rem 1rem' }}><StatusBadge status={svc.status || 'Active'} /></td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  )
}
