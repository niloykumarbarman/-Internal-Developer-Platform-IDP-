import { useState } from 'react'
import { useQuery } from '@tanstack/react-query'
import { apiClient } from '../../api/client'
import { ScrollText, Search, Filter, User, Shield, Zap, Database, Key } from 'lucide-react'

const mockLogs = [
  { id: '1', action: 'SERVICE_REGISTERED', resource: 'catalog/payment-service', user: 'jane.smith', role: 'PlatformEngineer', ip: '10.0.0.12', time: '2 min ago', status: 'Success' },
  { id: '2', action: 'SECRET_ROTATED', resource: 'vault/secret/idp/github', user: 'ci-bot', role: 'Admin', ip: '10.0.0.1', time: '15 min ago', status: 'Success' },
  { id: '3', action: 'NAMESPACE_PROVISIONED', resource: 'k8s/idp-dev-new', user: 'john.doe', role: 'PlatformEngineer', ip: '10.0.0.45', time: '32 min ago', status: 'Success' },
  { id: '4', action: 'PIPELINE_TRIGGERED', resource: 'cicd/frontend/main', user: 'bob.dev', role: 'Developer', ip: '10.0.0.88', time: '1h ago', status: 'Success' },
  { id: '5', action: 'LOGIN_FAILED', resource: 'auth/login', user: 'unknown', role: '-', ip: '192.168.1.100', time: '2h ago', status: 'Failed' },
  { id: '6', action: 'INCIDENT_CREATED', resource: 'incidents/INC-002', user: 'jane.smith', role: 'PlatformEngineer', ip: '10.0.0.12', time: '6h ago', status: 'Success' },
  { id: '7', action: 'USER_REGISTERED', resource: 'auth/users/alice', user: 'alice.new', role: 'Developer', ip: '10.0.1.55', time: '1d ago', status: 'Success' },
  { id: '8', action: 'SECRET_ACCESSED', resource: 'vault/secret/idp/jwt', user: 'api-gateway', role: 'Service', ip: '10.0.0.20', time: '1d ago', status: 'Success' },
]

const actionIcon: Record<string, any> = {
  SERVICE_REGISTERED: BookIcon, SECRET_ROTATED: Key, NAMESPACE_PROVISIONED: Shield,
  PIPELINE_TRIGGERED: Zap, LOGIN_FAILED: User, INCIDENT_CREATED: Shield,
  USER_REGISTERED: User, SECRET_ACCESSED: Key,
}

function BookIcon({ size, color }: any) { return <ScrollText size={size} color={color} /> }

const actionColor: Record<string, string> = {
  SERVICE_REGISTERED: 'oklch(0.6 0.18 150)', SECRET_ROTATED: 'oklch(0.75 0.18 80)',
  NAMESPACE_PROVISIONED: 'oklch(0.6 0.2 250)', PIPELINE_TRIGGERED: 'oklch(0.75 0.18 200)',
  LOGIN_FAILED: 'oklch(0.6 0.22 27)', INCIDENT_CREATED: 'oklch(0.6 0.22 27)',
  USER_REGISTERED: 'oklch(0.6 0.18 150)', SECRET_ACCESSED: 'oklch(0.75 0.18 80)',
}

export default function AuditPage() {
  const [search, setSearch] = useState('')
  const [filter, setFilter] = useState('All')

  const { data } = useQuery({
    queryKey: ['audit'],
    queryFn: () => apiClient.get('/audit').then(r => r.data),
    retry: false,
  })

  const logs = data?.items ?? mockLogs
  const filtered = logs.filter((l: any) =>
    (filter === 'All' || l.action.includes(filter)) &&
    (l.action.toLowerCase().includes(search.toLowerCase()) ||
     l.user.toLowerCase().includes(search.toLowerCase()) ||
     l.resource.toLowerCase().includes(search.toLowerCase()))
  )

  const inp: React.CSSProperties = { padding: '0.55rem 0.75rem', background: 'oklch(0.22 0 0)', border: '1px solid oklch(0.32 0 0)', borderRadius: 6, color: 'oklch(0.95 0 0)', fontSize: 13, outline: 'none' }

  return (
    <div style={{ display: 'flex', flexDirection: 'column', gap: '1.25rem' }}>
      <div>
        <h1 style={{ fontSize: 22, fontWeight: 700, color: 'oklch(0.95 0 0)', marginBottom: 4 }}>Audit Logs</h1>
        <p style={{ fontSize: 13, color: 'oklch(0.5 0 0)' }}>Activity Tracking · Compliance · Security Events</p>
      </div>

      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(4, 1fr)', gap: '0.75rem' }}>
        {[
          { label: 'Events Today', value: '284', color: 'oklch(0.6 0.2 250)' },
          { label: 'Failed Actions', value: '3', color: 'oklch(0.6 0.22 27)' },
          { label: 'Active Users', value: '12', color: 'oklch(0.6 0.18 150)' },
          { label: 'Security Events', value: '1', color: 'oklch(0.75 0.18 80)' },
        ].map(s => (
          <div key={s.label} style={{ background: 'oklch(0.18 0 0)', border: '1px solid oklch(0.28 0 0)', borderRadius: 8, padding: '1rem', textAlign: 'center' }}>
            <div style={{ fontSize: 26, fontWeight: 700, color: s.color }}>{s.value}</div>
            <div style={{ fontSize: 12, color: 'oklch(0.5 0 0)', marginTop: 4 }}>{s.label}</div>
          </div>
        ))}
      </div>

      {/* Filters */}
      <div style={{ display: 'flex', gap: 8, alignItems: 'center' }}>
        <div style={{ position: 'relative', flex: 1 }}>
          <Search size={14} style={{ position: 'absolute', left: 10, top: '50%', transform: 'translateY(-50%)', color: 'oklch(0.5 0 0)' }} />
          <input value={search} onChange={e => setSearch(e.target.value)} placeholder="Search actions, users, resources..." style={{ ...inp, paddingLeft: 32, width: '100%', boxSizing: 'border-box' }} />
        </div>
        <Filter size={14} color="oklch(0.5 0 0)" />
        {['All', 'SECRET', 'SERVICE', 'LOGIN', 'NAMESPACE', 'PIPELINE'].map(f => (
          <button key={f} onClick={() => setFilter(f)} style={{ padding: '0.45rem 0.75rem', background: filter === f ? 'oklch(0.6 0.2 250)' : 'oklch(0.22 0 0)', border: '1px solid oklch(0.32 0 0)', borderRadius: 6, color: filter === f ? '#fff' : 'oklch(0.65 0 0)', fontSize: 12, cursor: 'pointer', fontWeight: filter === f ? 600 : 400 }}>
            {f}
          </button>
        ))}
      </div>

      {/* Logs */}
      <div style={{ background: 'oklch(0.18 0 0)', border: '1px solid oklch(0.28 0 0)', borderRadius: 10, overflow: 'hidden' }}>
        <table style={{ width: '100%', borderCollapse: 'collapse' }}>
          <thead>
            <tr style={{ borderBottom: '1px solid oklch(0.25 0 0)' }}>
              {['Action', 'Resource', 'User', 'Role', 'IP', 'Time', 'Status'].map(h => (
                <th key={h} style={{ padding: '0.65rem 1rem', textAlign: 'left', fontSize: 11, fontWeight: 600, color: 'oklch(0.45 0 0)', textTransform: 'uppercase' }}>{h}</th>
              ))}
            </tr>
          </thead>
          <tbody>
            {filtered.map((log: any) => {
              const Icon = actionIcon[log.action] || ScrollText
              const color = actionColor[log.action] || 'oklch(0.55 0 0)'
              return (
                <tr key={log.id} style={{ borderBottom: '1px solid oklch(0.22 0 0)' }}>
                  <td style={{ padding: '0.75rem 1rem' }}>
                    <div style={{ display: 'flex', alignItems: 'center', gap: 8 }}>
                      <div style={{ width: 26, height: 26, borderRadius: 6, background: `${color.replace(')', ' / 0.12)')}`, display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
                        <Icon size={13} color={color} />
                      </div>
                      <span style={{ fontSize: 12, fontFamily: 'monospace', fontWeight: 600, color: 'oklch(0.82 0 0)' }}>{log.action}</span>
                    </div>
                  </td>
                  <td style={{ padding: '0.75rem 1rem', fontSize: 12, fontFamily: 'monospace', color: 'oklch(0.6 0 0)' }}>{log.resource}</td>
                  <td style={{ padding: '0.75rem 1rem', fontSize: 12, color: 'oklch(0.75 0 0)' }}>{log.user}</td>
                  <td style={{ padding: '0.75rem 1rem', fontSize: 11, color: 'oklch(0.55 0 0)' }}>{log.role}</td>
                  <td style={{ padding: '0.75rem 1rem', fontSize: 12, fontFamily: 'monospace', color: 'oklch(0.5 0 0)' }}>{log.ip}</td>
                  <td style={{ padding: '0.75rem 1rem', fontSize: 12, color: 'oklch(0.5 0 0)' }}>{log.time}</td>
                  <td style={{ padding: '0.75rem 1rem' }}>
                    <span style={{ fontSize: 11, fontWeight: 600, padding: '2px 8px', borderRadius: 20, color: log.status === 'Success' ? 'oklch(0.6 0.18 150)' : 'oklch(0.6 0.22 27)', background: log.status === 'Success' ? 'oklch(0.6 0.18 150 / 0.12)' : 'oklch(0.6 0.22 27 / 0.12)' }}>{log.status}</span>
                  </td>
                </tr>
              )
            })}
          </tbody>
        </table>
        <div style={{ padding: '0.75rem 1rem', borderTop: '1px solid oklch(0.22 0 0)', fontSize: 12, color: 'oklch(0.45 0 0)' }}>
          Showing {filtered.length} of {logs.length} events
        </div>
      </div>
    </div>
  )
}
