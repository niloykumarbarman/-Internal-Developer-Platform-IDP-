import { useState } from 'react'
import { useMutation } from '@tanstack/react-query'
import { apiClient } from '../../api/client'
import { AlertTriangle, Plus, Clock, CheckCircle, XCircle, Loader2, X } from 'lucide-react'

const incidents = [
  { id: 'INC-001', title: 'catalog-service high error rate', severity: 'High', status: 'Open', service: 'catalog-service', owner: 'Platform Team', created: '2h ago', timeline: ['14:00 - Alert fired', '14:05 - On-call notified', '14:15 - Root cause identified: memory leak', '14:30 - Fix deployed to staging'] },
  { id: 'INC-002', title: 'notification-svc pod crash loop', severity: 'Critical', status: 'Open', service: 'notification-svc', owner: 'Core Team', created: '6h ago', timeline: ['08:00 - Pod crash detected', '08:05 - Auto-restart triggered', '08:20 - Manual investigation started', '09:00 - Config error found'] },
  { id: 'INC-003', title: 'Auth service slow response', severity: 'Medium', status: 'Resolved', service: 'auth-service', owner: 'Security Team', created: '1d ago', timeline: ['Resolved after Redis cache flush'] },
  { id: 'INC-004', title: 'Database connection pool exhausted', severity: 'High', status: 'Resolved', service: 'api-gateway', owner: 'Platform Team', created: '3d ago', timeline: ['Resolved after increasing pool size'] },
]

const sevColor: Record<string, string> = { Critical: 'oklch(0.6 0.22 27)', High: 'oklch(0.75 0.18 50)', Medium: 'oklch(0.75 0.18 80)', Low: 'oklch(0.6 0.2 250)' }
const statusColor: Record<string, string> = { Open: 'oklch(0.6 0.22 27)', Resolved: 'oklch(0.6 0.18 150)', Investigating: 'oklch(0.75 0.18 80)' }

export default function IncidentPage() {
  const [selected, setSelected] = useState<any>(null)
  const [showForm, setShowForm] = useState(false)
  const [form, setForm] = useState({ title: '', severity: 'Medium', service: '', description: '' })

  const createMutation = useMutation({
    mutationFn: () => apiClient.post('/incidents', form),
    onSuccess: () => { setShowForm(false); setForm({ title: '', severity: 'Medium', service: '', description: '' }) },
  })

  const inp: React.CSSProperties = { width: '100%', padding: '0.55rem 0.75rem', background: 'oklch(0.22 0 0)', border: '1px solid oklch(0.32 0 0)', borderRadius: 6, color: 'oklch(0.95 0 0)', fontSize: 13, outline: 'none', boxSizing: 'border-box' }

  return (
    <div style={{ display: 'flex', flexDirection: 'column', gap: '1.25rem' }}>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start' }}>
        <div>
          <h1 style={{ fontSize: 22, fontWeight: 700, color: 'oklch(0.95 0 0)', marginBottom: 4 }}>Incident Management</h1>
          <p style={{ fontSize: 13, color: 'oklch(0.5 0 0)' }}>Alert Dashboard · Timeline · Root Cause · Postmortem</p>
        </div>
        <button onClick={() => setShowForm(true)} style={{ display: 'flex', alignItems: 'center', gap: 6, padding: '0.55rem 1rem', background: 'oklch(0.6 0.22 27)', border: 'none', borderRadius: 6, color: '#fff', fontSize: 13, fontWeight: 600, cursor: 'pointer' }}>
          <Plus size={14} /> Report Incident
        </button>
      </div>

      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(4, 1fr)', gap: '0.75rem' }}>
        {[
          { label: 'Open', value: incidents.filter(i => i.status === 'Open').length, color: 'oklch(0.6 0.22 27)' },
          { label: 'Critical', value: incidents.filter(i => i.severity === 'Critical').length, color: 'oklch(0.6 0.22 27)' },
          { label: 'Resolved Today', value: '2', color: 'oklch(0.6 0.18 150)' },
          { label: 'Avg MTTR', value: '42m', color: 'oklch(0.6 0.2 250)' },
        ].map(s => (
          <div key={s.label} style={{ background: 'oklch(0.18 0 0)', border: '1px solid oklch(0.28 0 0)', borderRadius: 8, padding: '1rem', textAlign: 'center' }}>
            <div style={{ fontSize: 26, fontWeight: 700, color: s.color }}>{s.value}</div>
            <div style={{ fontSize: 12, color: 'oklch(0.5 0 0)', marginTop: 4 }}>{s.label}</div>
          </div>
        ))}
      </div>

      <div style={{ display: 'grid', gridTemplateColumns: selected ? '1fr 380px' : '1fr', gap: '1rem' }}>
        <div style={{ display: 'flex', flexDirection: 'column', gap: 8 }}>
          {incidents.map(inc => (
            <div key={inc.id} onClick={() => setSelected(selected?.id === inc.id ? null : inc)}
              style={{ background: 'oklch(0.18 0 0)', border: `1px solid ${selected?.id === inc.id ? sevColor[inc.severity] : 'oklch(0.28 0 0)'}`, borderRadius: 10, padding: '1rem', cursor: 'pointer', borderLeft: `4px solid ${sevColor[inc.severity]}` }}>
              <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
                <div style={{ display: 'flex', alignItems: 'center', gap: 10 }}>
                  <AlertTriangle size={16} color={sevColor[inc.severity]} />
                  <div>
                    <div style={{ fontSize: 14, fontWeight: 600, color: 'oklch(0.9 0 0)' }}>{inc.title}</div>
                    <div style={{ fontSize: 12, color: 'oklch(0.5 0 0)' }}>{inc.id} · {inc.service} · {inc.owner} · {inc.created}</div>
                  </div>
                </div>
                <div style={{ display: 'flex', gap: 6 }}>
                  <span style={{ fontSize: 11, fontWeight: 600, padding: '2px 8px', borderRadius: 20, color: sevColor[inc.severity], background: `${sevColor[inc.severity].replace(')', ' / 0.12)')}` }}>{inc.severity}</span>
                  <span style={{ fontSize: 11, fontWeight: 600, padding: '2px 8px', borderRadius: 20, color: statusColor[inc.status], background: `${statusColor[inc.status].replace(')', ' / 0.12)')}` }}>{inc.status}</span>
                </div>
              </div>
            </div>
          ))}
        </div>

        {selected && (
          <div style={{ background: 'oklch(0.18 0 0)', border: '1px solid oklch(0.28 0 0)', borderRadius: 10, padding: '1.25rem', height: 'fit-content' }}>
            <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: '1rem' }}>
              <div style={{ fontSize: 14, fontWeight: 600, color: 'oklch(0.85 0 0)' }}>Incident Timeline</div>
              <button onClick={() => setSelected(null)} style={{ background: 'none', border: 'none', cursor: 'pointer', color: 'oklch(0.5 0 0)' }}><X size={16} /></button>
            </div>
            <div style={{ fontSize: 13, fontWeight: 600, color: 'oklch(0.9 0 0)', marginBottom: 4 }}>{selected.title}</div>
            <div style={{ fontSize: 12, color: 'oklch(0.5 0 0)', marginBottom: '1rem' }}>{selected.id} · {selected.service}</div>
            <div style={{ display: 'flex', flexDirection: 'column', gap: 8 }}>
              {selected.timeline.map((t: string, i: number) => (
                <div key={i} style={{ display: 'flex', gap: 10, alignItems: 'flex-start' }}>
                  <div style={{ width: 8, height: 8, borderRadius: '50%', background: 'oklch(0.6 0.2 250)', marginTop: 4, flexShrink: 0 }} />
                  <div style={{ fontSize: 12, color: 'oklch(0.7 0 0)' }}>{t}</div>
                </div>
              ))}
            </div>
          </div>
        )}
      </div>

      {showForm && (
        <div style={{ position: 'fixed', inset: 0, background: 'rgba(0,0,0,0.7)', display: 'flex', alignItems: 'center', justifyContent: 'center', zIndex: 50 }}>
          <div style={{ background: 'oklch(0.18 0 0)', border: '1px solid oklch(0.3 0 0)', borderRadius: 12, padding: '1.5rem', width: 400 }}>
            <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: '1.25rem' }}>
              <h2 style={{ fontSize: 16, fontWeight: 700, color: 'oklch(0.95 0 0)' }}>Report Incident</h2>
              <button onClick={() => setShowForm(false)} style={{ background: 'none', border: 'none', cursor: 'pointer', color: 'oklch(0.5 0 0)' }}><X size={18} /></button>
            </div>
            <div style={{ display: 'flex', flexDirection: 'column', gap: '0.85rem' }}>
              <div><label style={{ display: 'block', fontSize: 12, color: 'oklch(0.6 0 0)', marginBottom: 4 }}>Title</label><input style={inp} value={form.title} onChange={e => setForm({ ...form, title: e.target.value })} placeholder="Brief description" /></div>
              <div><label style={{ display: 'block', fontSize: 12, color: 'oklch(0.6 0 0)', marginBottom: 4 }}>Affected Service</label><input style={inp} value={form.service} onChange={e => setForm({ ...form, service: e.target.value })} placeholder="service-name" /></div>
              <div>
                <label style={{ display: 'block', fontSize: 12, color: 'oklch(0.6 0 0)', marginBottom: 4 }}>Severity</label>
                <select style={{ ...inp, cursor: 'pointer' }} value={form.severity} onChange={e => setForm({ ...form, severity: e.target.value })}>
                  {['Low', 'Medium', 'High', 'Critical'].map(s => <option key={s}>{s}</option>)}
                </select>
              </div>
              <div><label style={{ display: 'block', fontSize: 12, color: 'oklch(0.6 0 0)', marginBottom: 4 }}>Description</label><textarea style={{ ...inp, height: 70, resize: 'none' }} value={form.description} onChange={e => setForm({ ...form, description: e.target.value })} placeholder="What happened?" /></div>
              <button onClick={() => createMutation.mutate()} disabled={!form.title || createMutation.isPending} style={{ padding: '0.65rem', background: 'oklch(0.6 0.22 27)', border: 'none', borderRadius: 6, color: '#fff', fontSize: 13, fontWeight: 600, cursor: 'pointer', display: 'flex', alignItems: 'center', justifyContent: 'center', gap: 6 }}>
                {createMutation.isPending ? <Loader2 size={14} /> : <AlertTriangle size={14} />} Create Incident
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  )
}
