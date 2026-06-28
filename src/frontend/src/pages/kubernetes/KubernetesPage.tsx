import { useState } from 'react'
import { useMutation } from '@tanstack/react-query'
import { apiClient } from '../../api/client'
import { Layers, Plus, Cpu, MemoryStick, Activity, CheckCircle, Loader2 } from 'lucide-react'

const namespaces = [
  { name: 'idp-dev', env: 'Development', pods: 12, cpu: '1.2/4', mem: '2.1/8GB', status: 'Active' },
  { name: 'idp-staging', env: 'Staging', pods: 8, cpu: '0.8/4', mem: '1.5/8GB', status: 'Active' },
  { name: 'idp-prod', env: 'Production', pods: 24, cpu: '3.2/8', mem: '6.4/16GB', status: 'Active' },
  { name: 'monitoring', env: 'Platform', pods: 6, cpu: '0.5/2', mem: '1.8/4GB', status: 'Active' },
  { name: 'vault', env: 'Platform', pods: 3, cpu: '0.2/1', mem: '0.5/2GB', status: 'Active' },
]

const pods = [
  { name: 'api-gateway-7d9f8c-xk2p', namespace: 'idp-prod', status: 'Running', restarts: 0, age: '5d', cpu: '120m', mem: '256Mi' },
  { name: 'auth-service-5b6c7d-mn3q', namespace: 'idp-prod', status: 'Running', restarts: 0, age: '5d', cpu: '80m', mem: '128Mi' },
  { name: 'catalog-svc-3a4b5c-pq7r', namespace: 'idp-prod', status: 'Running', restarts: 2, age: '2d', cpu: '200m', mem: '512Mi' },
  { name: 'frontend-6e7f8g-rs9t', namespace: 'idp-prod', status: 'Running', restarts: 0, age: '1d', cpu: '50m', mem: '64Mi' },
  { name: 'payment-svc-1b2c3d-uv0w', namespace: 'idp-staging', status: 'Running', restarts: 1, age: '3d', cpu: '150m', mem: '320Mi' },
  { name: 'notif-svc-9h0i1j-wx2y', namespace: 'idp-dev', status: 'CrashLoop', restarts: 8, age: '6h', cpu: '10m', mem: '32Mi' },
]

const statusC: Record<string, string> = { Running: 'oklch(0.6 0.18 150)', CrashLoop: 'oklch(0.6 0.22 27)', Pending: 'oklch(0.75 0.18 80)', Active: 'oklch(0.6 0.18 150)' }

export default function KubernetesPage() {
  const [tab, setTab] = useState<'namespaces' | 'pods'>('namespaces')
  const [showForm, setShowForm] = useState(false)
  const [nsName, setNsName] = useState('')
  const [env, setEnv] = useState('Development')

  const provMutation = useMutation({
    mutationFn: () => apiClient.post('/kubernetes/namespace', { namespaceName: nsName, environment: env }),
    onSuccess: () => { setShowForm(false); setNsName('') },
  })

  const inp: React.CSSProperties = { width: '100%', padding: '0.55rem 0.75rem', background: 'oklch(0.22 0 0)', border: '1px solid oklch(0.32 0 0)', borderRadius: 6, color: 'oklch(0.95 0 0)', fontSize: 13, outline: 'none', boxSizing: 'border-box' }

  return (
    <div style={{ display: 'flex', flexDirection: 'column', gap: '1.25rem' }}>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start' }}>
        <div>
          <h1 style={{ fontSize: 22, fontWeight: 700, color: 'oklch(0.95 0 0)', marginBottom: 4 }}>Kubernetes Platform</h1>
          <p style={{ fontSize: 13, color: 'oklch(0.5 0 0)' }}>Namespace · Pods · HPA · Resource Quotas</p>
        </div>
        <button onClick={() => setShowForm(true)} style={{ display: 'flex', alignItems: 'center', gap: 6, padding: '0.55rem 1rem', background: 'oklch(0.6 0.2 250)', border: 'none', borderRadius: 6, color: '#fff', fontSize: 13, fontWeight: 600, cursor: 'pointer' }}>
          <Plus size={14} /> Provision Namespace
        </button>
      </div>

      {/* Cluster Stats */}
      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(4, 1fr)', gap: '0.75rem' }}>
        {[
          { label: 'Total Namespaces', value: '8', icon: Layers, color: 'oklch(0.6 0.2 250)' },
          { label: 'Running Pods', value: '53', icon: Activity, color: 'oklch(0.6 0.18 150)' },
          { label: 'CPU Usage', value: '62%', icon: Cpu, color: 'oklch(0.75 0.18 80)' },
          { label: 'Memory Usage', value: '71%', icon: MemoryStick, color: 'oklch(0.75 0.18 200)' },
        ].map(s => (
          <div key={s.label} style={{ background: 'oklch(0.18 0 0)', border: '1px solid oklch(0.28 0 0)', borderRadius: 8, padding: '1rem', display: 'flex', alignItems: 'center', gap: 12 }}>
            <div style={{ width: 36, height: 36, borderRadius: 8, background: `${s.color.replace(')', ' / 0.12)')}`, display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
              <s.icon size={18} color={s.color} />
            </div>
            <div>
              <div style={{ fontSize: 20, fontWeight: 700, color: 'oklch(0.95 0 0)' }}>{s.value}</div>
              <div style={{ fontSize: 11, color: 'oklch(0.5 0 0)' }}>{s.label}</div>
            </div>
          </div>
        ))}
      </div>

      {/* Tabs */}
      <div style={{ display: 'flex', gap: 4, borderBottom: '1px solid oklch(0.25 0 0)', paddingBottom: 0 }}>
        {(['namespaces', 'pods'] as const).map(t => (
          <button key={t} onClick={() => setTab(t)} style={{ padding: '0.5rem 1rem', background: 'none', border: 'none', borderBottom: tab === t ? '2px solid oklch(0.6 0.2 250)' : '2px solid transparent', color: tab === t ? 'oklch(0.9 0 0)' : 'oklch(0.5 0 0)', fontSize: 13, fontWeight: 600, cursor: 'pointer', textTransform: 'capitalize' }}>
            {t}
          </button>
        ))}
      </div>

      {tab === 'namespaces' && (
        <div style={{ display: 'grid', gridTemplateColumns: 'repeat(2, 1fr)', gap: '0.75rem' }}>
          {namespaces.map(ns => (
            <div key={ns.name} style={{ background: 'oklch(0.18 0 0)', border: '1px solid oklch(0.28 0 0)', borderRadius: 10, padding: '1rem' }}>
              <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: 10 }}>
                <div>
                  <div style={{ fontSize: 14, fontWeight: 600, color: 'oklch(0.9 0 0)' }}>{ns.name}</div>
                  <div style={{ fontSize: 12, color: 'oklch(0.5 0 0)' }}>{ns.env}</div>
                </div>
                <span style={{ fontSize: 11, fontWeight: 600, padding: '2px 8px', borderRadius: 20, color: statusC[ns.status], background: `${statusC[ns.status].replace(')', ' / 0.12)')}` }}>{ns.status}</span>
              </div>
              <div style={{ display: 'grid', gridTemplateColumns: 'repeat(3, 1fr)', gap: 6 }}>
                {[['Pods', ns.pods], ['CPU', ns.cpu], ['Memory', ns.mem]].map(([l, v]) => (
                  <div key={l as string} style={{ background: 'oklch(0.22 0 0)', borderRadius: 6, padding: '0.4rem 0.6rem', textAlign: 'center' }}>
                    <div style={{ fontSize: 13, fontWeight: 600, color: 'oklch(0.85 0 0)' }}>{v}</div>
                    <div style={{ fontSize: 11, color: 'oklch(0.5 0 0)' }}>{l}</div>
                  </div>
                ))}
              </div>
            </div>
          ))}
        </div>
      )}

      {tab === 'pods' && (
        <div style={{ background: 'oklch(0.18 0 0)', border: '1px solid oklch(0.28 0 0)', borderRadius: 10, overflow: 'hidden' }}>
          <table style={{ width: '100%', borderCollapse: 'collapse' }}>
            <thead>
              <tr style={{ borderBottom: '1px solid oklch(0.25 0 0)' }}>
                {['Pod Name', 'Namespace', 'Status', 'Restarts', 'CPU', 'Memory', 'Age'].map(h => (
                  <th key={h} style={{ padding: '0.65rem 0.85rem', textAlign: 'left', fontSize: 11, fontWeight: 600, color: 'oklch(0.5 0 0)', textTransform: 'uppercase' }}>{h}</th>
                ))}
              </tr>
            </thead>
            <tbody>
              {pods.map(p => (
                <tr key={p.name} style={{ borderBottom: '1px solid oklch(0.22 0 0)' }}>
                  <td style={{ padding: '0.75rem 0.85rem', fontSize: 12, fontWeight: 500, color: 'oklch(0.85 0 0)', fontFamily: 'monospace' }}>{p.name}</td>
                  <td style={{ padding: '0.75rem 0.85rem', fontSize: 12, color: 'oklch(0.6 0 0)' }}>{p.namespace}</td>
                  <td style={{ padding: '0.75rem 0.85rem' }}>
                    <span style={{ fontSize: 11, fontWeight: 600, padding: '2px 8px', borderRadius: 20, color: statusC[p.status] || 'oklch(0.55 0 0)', background: `${(statusC[p.status] || 'oklch(0.55 0 0)').replace(')', ' / 0.12)')}` }}>{p.status}</span>
                  </td>
                  <td style={{ padding: '0.75rem 0.85rem', fontSize: 12, color: p.restarts > 3 ? 'oklch(0.6 0.22 27)' : 'oklch(0.6 0 0)' }}>{p.restarts}</td>
                  <td style={{ padding: '0.75rem 0.85rem', fontSize: 12, color: 'oklch(0.6 0 0)', fontFamily: 'monospace' }}>{p.cpu}</td>
                  <td style={{ padding: '0.75rem 0.85rem', fontSize: 12, color: 'oklch(0.6 0 0)', fontFamily: 'monospace' }}>{p.mem}</td>
                  <td style={{ padding: '0.75rem 0.85rem', fontSize: 12, color: 'oklch(0.5 0 0)' }}>{p.age}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}

      {showForm && (
        <div style={{ position: 'fixed', inset: 0, background: 'rgba(0,0,0,0.7)', display: 'flex', alignItems: 'center', justifyContent: 'center', zIndex: 50 }}>
          <div style={{ background: 'oklch(0.18 0 0)', border: '1px solid oklch(0.3 0 0)', borderRadius: 12, padding: '1.5rem', width: 360 }}>
            <h2 style={{ fontSize: 16, fontWeight: 700, color: 'oklch(0.95 0 0)', marginBottom: '1.25rem' }}>Provision Namespace</h2>
            <div style={{ display: 'flex', flexDirection: 'column', gap: '0.85rem' }}>
              <div>
                <label style={{ display: 'block', fontSize: 12, color: 'oklch(0.6 0 0)', marginBottom: 4 }}>Namespace Name</label>
                <input style={inp} value={nsName} onChange={e => setNsName(e.target.value)} placeholder="my-team-dev" />
              </div>
              <div>
                <label style={{ display: 'block', fontSize: 12, color: 'oklch(0.6 0 0)', marginBottom: 4 }}>Environment</label>
                <select style={{ ...inp, cursor: 'pointer' }} value={env} onChange={e => setEnv(e.target.value)}>
                  {['Development', 'Staging', 'Production'].map(e => <option key={e}>{e}</option>)}
                </select>
              </div>
              <div style={{ display: 'flex', gap: 8 }}>
                <button onClick={() => setShowForm(false)} style={{ flex: 1, padding: '0.6rem', background: 'oklch(0.25 0 0)', border: 'none', borderRadius: 6, color: 'oklch(0.7 0 0)', fontSize: 13, cursor: 'pointer' }}>Cancel</button>
                <button onClick={() => provMutation.mutate()} disabled={!nsName || provMutation.isPending} style={{ flex: 1, padding: '0.6rem', background: 'oklch(0.6 0.2 250)', border: 'none', borderRadius: 6, color: '#fff', fontSize: 13, fontWeight: 600, cursor: 'pointer', display: 'flex', alignItems: 'center', justifyContent: 'center', gap: 6 }}>
                  {provMutation.isPending ? <Loader2 size={13} /> : <CheckCircle size={13} />} Provision
                </button>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  )
}
