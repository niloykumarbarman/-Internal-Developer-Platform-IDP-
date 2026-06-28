import { useState } from 'react'
import { useQuery, useMutation } from '@tanstack/react-query'
import { apiClient } from '../../api/client'
import { Play, RefreshCw, CheckCircle, XCircle, Clock, GitBranch, Zap, Loader2 } from 'lucide-react'

const mockPipelines = [
  { id: '1', name: 'api-gateway', branch: 'main', status: 'Success', duration: '3m 42s', triggeredBy: 'john.doe', time: '10 min ago', steps: ['Build ✅', 'Test ✅', 'Scan ✅', 'Deploy ✅'] },
  { id: '2', name: 'auth-service', branch: 'release/v2', status: 'Running', duration: '1m 12s', triggeredBy: 'jane.smith', time: '2 min ago', steps: ['Build ✅', 'Test ✅', 'Scan 🔄', 'Deploy ⏳'] },
  { id: '3', name: 'frontend', branch: 'feature/ui', status: 'Failed', duration: '2m 05s', triggeredBy: 'bob.dev', time: '1 hour ago', steps: ['Build ✅', 'Test ❌', 'Scan ⏳', 'Deploy ⏳'] },
  { id: '4', name: 'catalog-service', branch: 'main', status: 'Pending', duration: '-', triggeredBy: 'ci-bot', time: 'queued', steps: ['Build ⏳', 'Test ⏳', 'Scan ⏳', 'Deploy ⏳'] },
  { id: '5', name: 'payment-service', branch: 'main', status: 'Success', duration: '4m 18s', triggeredBy: 'jane.smith', time: '3 hours ago', steps: ['Build ✅', 'Test ✅', 'Scan ✅', 'Deploy ✅'] },
]

const statusIcon = (s: string) => {
  if (s === 'Success') return <CheckCircle size={15} color="oklch(0.6 0.18 150)" />
  if (s === 'Failed') return <XCircle size={15} color="oklch(0.6 0.22 27)" />
  if (s === 'Running') return <RefreshCw size={15} color="oklch(0.6 0.2 250)" />
  return <Clock size={15} color="oklch(0.75 0.18 80)" />
}

const statusColor: Record<string, string> = {
  Success: 'oklch(0.6 0.18 150)', Failed: 'oklch(0.6 0.22 27)',
  Running: 'oklch(0.6 0.2 250)', Pending: 'oklch(0.75 0.18 80)',
}

export default function CicdPage() {
  const [selected, setSelected] = useState<any>(null)
  const [triggerName, setTriggerName] = useState('')

  const triggerMutation = useMutation({
    mutationFn: (name: string) => apiClient.post('/cicd/trigger', { serviceName: name, branch: 'main' }),
    onSuccess: () => setTriggerName(''),
  })

  const inp: React.CSSProperties = { padding: '0.55rem 0.75rem', background: 'oklch(0.22 0 0)', border: '1px solid oklch(0.32 0 0)', borderRadius: 6, color: 'oklch(0.95 0 0)', fontSize: 13, outline: 'none' }

  return (
    <div style={{ display: 'flex', flexDirection: 'column', gap: '1.25rem' }}>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start' }}>
        <div>
          <h1 style={{ fontSize: 22, fontWeight: 700, color: 'oklch(0.95 0 0)', marginBottom: 4 }}>CI/CD Pipelines</h1>
          <p style={{ fontSize: 13, color: 'oklch(0.5 0 0)' }}>GitHub Actions — Build · Test · Scan · Deploy</p>
        </div>
        <div style={{ display: 'flex', gap: 8 }}>
          <input value={triggerName} onChange={e => setTriggerName(e.target.value)} placeholder="service-name" style={{ ...inp, width: 160 }} />
          <button onClick={() => triggerMutation.mutate(triggerName)} disabled={!triggerName || triggerMutation.isPending}
            style={{ display: 'flex', alignItems: 'center', gap: 6, padding: '0.55rem 1rem', background: 'oklch(0.6 0.2 250)', border: 'none', borderRadius: 6, color: '#fff', fontSize: 13, fontWeight: 600, cursor: 'pointer' }}>
            {triggerMutation.isPending ? <Loader2 size={14} /> : <Play size={14} />} Trigger
          </button>
        </div>
      </div>

      {/* Stats */}
      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(4, 1fr)', gap: '0.75rem' }}>
        {[
          { label: 'Total Runs Today', value: '28', color: 'oklch(0.6 0.2 250)' },
          { label: 'Success Rate', value: '89%', color: 'oklch(0.6 0.18 150)' },
          { label: 'Avg Duration', value: '3m 12s', color: 'oklch(0.75 0.18 80)' },
          { label: 'Failed', value: '3', color: 'oklch(0.6 0.22 27)' },
        ].map(s => (
          <div key={s.label} style={{ background: 'oklch(0.18 0 0)', border: '1px solid oklch(0.28 0 0)', borderRadius: 8, padding: '1rem', textAlign: 'center' }}>
            <div style={{ fontSize: 24, fontWeight: 700, color: s.color }}>{s.value}</div>
            <div style={{ fontSize: 12, color: 'oklch(0.5 0 0)', marginTop: 4 }}>{s.label}</div>
          </div>
        ))}
      </div>

      {/* Pipeline list */}
      <div style={{ display: 'flex', flexDirection: 'column', gap: 8 }}>
        {mockPipelines.map(p => (
          <div key={p.id} onClick={() => setSelected(selected?.id === p.id ? null : p)}
            style={{ background: 'oklch(0.18 0 0)', border: `1px solid ${selected?.id === p.id ? 'oklch(0.6 0.2 250)' : 'oklch(0.28 0 0)'}`, borderRadius: 10, padding: '1rem', cursor: 'pointer' }}>
            <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
              <div style={{ display: 'flex', alignItems: 'center', gap: 12 }}>
                {statusIcon(p.status)}
                <div>
                  <div style={{ fontSize: 14, fontWeight: 600, color: 'oklch(0.9 0 0)' }}>{p.name}</div>
                  <div style={{ fontSize: 12, color: 'oklch(0.5 0 0)', display: 'flex', alignItems: 'center', gap: 6, marginTop: 2 }}>
                    <GitBranch size={11} /> {p.branch} · {p.triggeredBy} · {p.time}
                  </div>
                </div>
              </div>
              <div style={{ display: 'flex', alignItems: 'center', gap: 12 }}>
                <span style={{ fontSize: 12, color: 'oklch(0.5 0 0)' }}>{p.duration}</span>
                <span style={{ fontSize: 12, fontWeight: 600, padding: '2px 10px', borderRadius: 20, color: statusColor[p.status], background: `${statusColor[p.status].replace(')', ' / 0.12)')}` }}>{p.status}</span>
              </div>
            </div>
            {selected?.id === p.id && (
              <div style={{ marginTop: '0.75rem', paddingTop: '0.75rem', borderTop: '1px solid oklch(0.25 0 0)', display: 'flex', gap: '0.5rem' }}>
                {p.steps.map((step, i) => (
                  <div key={i} style={{ flex: 1, background: 'oklch(0.22 0 0)', borderRadius: 6, padding: '0.4rem 0.6rem', fontSize: 12, color: 'oklch(0.7 0 0)', textAlign: 'center' }}>{step}</div>
                ))}
              </div>
            )}
          </div>
        ))}
      </div>
    </div>
  )
}
