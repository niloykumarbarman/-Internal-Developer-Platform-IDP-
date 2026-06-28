import { useState } from 'react'
import { Rocket, Code2, Database, Globe, GitBranch, CheckCircle2, ChevronRight, Layers } from 'lucide-react'

const templates = [
  { id: 'react-app', name: 'React Application', desc: 'Vite + React + TypeScript + Tailwind', icon: Globe, color: 'oklch(0.6 0.2 200)', tags: ['Frontend', 'TypeScript'] },
  { id: 'dotnet-api', name: 'ASP.NET Core API', desc: 'Clean Architecture + DDD + CQRS', icon: Code2, color: 'oklch(0.6 0.18 150)', tags: ['Backend', 'C#'] },
  { id: 'microservice', name: 'Microservice', desc: 'Docker + Kubernetes ready microservice', icon: Layers, color: 'oklch(0.6 0.2 250)', tags: ['Backend', 'Docker'] },
  { id: 'postgres-service', name: 'PostgreSQL Service', desc: 'Database service with migrations', icon: Database, color: 'oklch(0.65 0.18 80)', tags: ['Database', 'PostgreSQL'] },
  { id: 'fullstack', name: 'Full Stack App', desc: 'React frontend + .NET API + PostgreSQL', icon: GitBranch, color: 'oklch(0.6 0.2 300)', tags: ['Full Stack', 'TypeScript', 'C#'] },
]

const steps = ['Choose Template', 'Configure', 'Review & Create']

export default function SelfServicePage() {
  const [step, setStep] = useState(0)
  const [selected, setSelected] = useState<string | null>(null)
  const [form, setForm] = useState({ name: '', description: '', team: '', environment: 'development', repo: true, cicd: true, k8s: true })
  const [created, setCreated] = useState(false)
  const [loading, setLoading] = useState(false)

  const handleCreate = async () => {
    setLoading(true)
    await new Promise(r => setTimeout(r, 2000))
    setLoading(false)
    setCreated(true)
  }

  if (created) return (
    <div style={{ padding: '2rem', display: 'flex', flexDirection: 'column', alignItems: 'center', justifyContent: 'center', minHeight: '60vh', gap: '1rem' }}>
      <div style={{ width: 64, height: 64, borderRadius: '50%', background: 'oklch(0.6 0.18 150 / 0.15)', display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
        <CheckCircle2 size={36} color='oklch(0.6 0.18 150)' />
      </div>
      <h2 style={{ fontSize: 24, fontWeight: 700, color: 'oklch(0.985 0 0)', margin: 0 }}>Service Created!</h2>
      <p style={{ color: 'oklch(0.6 0 0)', margin: 0 }}>
        <b style={{ color: 'oklch(0.985 0 0)' }}>{form.name}</b> has been scaffolded successfully.
      </p>
      <div style={{ display: 'flex', gap: '0.75rem', flexWrap: 'wrap', justifyContent: 'center' }}>
        {form.repo && <span style={{ padding: '0.35rem 0.75rem', borderRadius: 20, background: 'oklch(0.6 0.2 250 / 0.15)', color: 'oklch(0.6 0.2 250)', fontSize: 12 }}>✓ GitHub Repo Created</span>}
        {form.cicd && <span style={{ padding: '0.35rem 0.75rem', borderRadius: 20, background: 'oklch(0.6 0.18 150 / 0.15)', color: 'oklch(0.6 0.18 150)', fontSize: 12 }}>✓ CI/CD Pipeline Ready</span>}
        {form.k8s && <span style={{ padding: '0.35rem 0.75rem', borderRadius: 20, background: 'oklch(0.65 0.18 80 / 0.15)', color: 'oklch(0.65 0.18 80)', fontSize: 12 }}>✓ K8s Namespace Provisioned</span>}
      </div>
      <button onClick={() => { setCreated(false); setStep(0); setSelected(null); setForm({ name: '', description: '', team: '', environment: 'development', repo: true, cicd: true, k8s: true }) }}
        style={{ marginTop: '1rem', padding: '0.6rem 1.5rem', borderRadius: 8, background: 'oklch(0.6 0.2 250)', color: '#fff', border: 'none', cursor: 'pointer', fontWeight: 600 }}>
        Create Another
      </button>
    </div>
  )

  return (
    <div style={{ padding: '1.5rem', maxWidth: 900, margin: '0 auto' }}>
      <div style={{ marginBottom: '1.5rem' }}>
        <h1 style={{ fontSize: 22, fontWeight: 700, color: 'oklch(0.985 0 0)', margin: '0 0 0.25rem', display: 'flex', alignItems: 'center', gap: '0.5rem' }}>
          <Rocket size={22} color='oklch(0.6 0.2 250)' /> Self-Service Portal
        </h1>
        <p style={{ color: 'oklch(0.55 0 0)', margin: 0, fontSize: 13 }}>Scaffold new services using Golden Path Templates</p>
      </div>

      {/* Stepper */}
      <div style={{ display: 'flex', alignItems: 'center', gap: '0.5rem', marginBottom: '2rem' }}>
        {steps.map((s, i) => (
          <div key={s} style={{ display: 'flex', alignItems: 'center', gap: '0.5rem' }}>
            <div style={{ display: 'flex', alignItems: 'center', gap: '0.5rem' }}>
              <div style={{ width: 28, height: 28, borderRadius: '50%', display: 'flex', alignItems: 'center', justifyContent: 'center', fontSize: 12, fontWeight: 700,
                background: i <= step ? 'oklch(0.6 0.2 250)' : 'oklch(0.22 0 0)', color: i <= step ? '#fff' : 'oklch(0.5 0 0)' }}>{i + 1}</div>
              <span style={{ fontSize: 13, color: i === step ? 'oklch(0.985 0 0)' : 'oklch(0.5 0 0)', fontWeight: i === step ? 600 : 400 }}>{s}</span>
            </div>
            {i < steps.length - 1 && <ChevronRight size={14} color='oklch(0.35 0 0)' />}
          </div>
        ))}
      </div>

      {/* Step 0: Choose Template */}
      {step === 0 && (
        <div>
          <p style={{ color: 'oklch(0.6 0 0)', fontSize: 13, marginBottom: '1rem' }}>Select a Golden Path Template to get started:</p>
          <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fill, minmax(250px, 1fr))', gap: '1rem' }}>
            {templates.map(t => {
              const Icon = t.icon
              const isSelected = selected === t.id
              return (
                <div key={t.id} onClick={() => setSelected(t.id)} style={{
                  padding: '1.25rem', borderRadius: 10, cursor: 'pointer', transition: 'all 0.15s',
                  background: isSelected ? 'oklch(0.6 0.2 250 / 0.1)' : 'oklch(0.14 0 0)',
                  border: `1.5px solid ${isSelected ? 'oklch(0.6 0.2 250)' : 'oklch(0.22 0 0)'}`,
                }}>
                  <div style={{ width: 40, height: 40, borderRadius: 8, background: `${t.color}22`, display: "flex", alignItems: "center", justifyContent: "center", marginBottom: "0.75rem" }}>
                    <Icon size={20} color={t.color} />
                  </div>
                  <div style={{ fontWeight: 600, color: 'oklch(0.985 0 0)', fontSize: 14, marginBottom: 4 }}>{t.name}</div>
                  <div style={{ color: 'oklch(0.55 0 0)', fontSize: 12, marginBottom: '0.75rem' }}>{t.desc}</div>
                  <div style={{ display: 'flex', gap: '0.4rem', flexWrap: 'wrap' }}>
                    {t.tags.map(tag => <span key={tag} style={{ padding: '0.2rem 0.5rem', borderRadius: 4, background: 'oklch(0.22 0 0)', color: 'oklch(0.6 0 0)', fontSize: 11 }}>{tag}</span>)}
                  </div>
                </div>
              )
            })}
          </div>
          <div style={{ marginTop: '1.5rem', display: 'flex', justifyContent: 'flex-end' }}>
            <button disabled={!selected} onClick={() => setStep(1)} style={{ padding: '0.6rem 1.5rem', borderRadius: 8, background: selected ? 'oklch(0.6 0.2 250)' : 'oklch(0.25 0 0)', color: selected ? '#fff' : 'oklch(0.45 0 0)', border: 'none', cursor: selected ? 'pointer' : 'not-allowed', fontWeight: 600, fontSize: 14 }}>
              Next →
            </button>
          </div>
        </div>
      )}

      {/* Step 1: Configure */}
      {step === 1 && (
        <div style={{ background: 'oklch(0.14 0 0)', border: '1px solid oklch(0.22 0 0)', borderRadius: 10, padding: '1.5rem' }}>
          <h3 style={{ color: 'oklch(0.985 0 0)', margin: '0 0 1.25rem', fontSize: 16 }}>Configure Your Service</h3>
          <div style={{ display: 'grid', gap: '1rem' }}>
            {[
              { label: 'Service Name *', key: 'name', placeholder: 'my-awesome-service' },
              { label: 'Description', key: 'description', placeholder: 'What does this service do?' },
              { label: 'Team', key: 'team', placeholder: 'e.g. platform-team' },
            ].map(({ label, key, placeholder }) => (
              <div key={key}>
                <label style={{ display: 'block', fontSize: 12, color: 'oklch(0.6 0 0)', marginBottom: 6 }}>{label}</label>
                <input value={(form as any)[key]} onChange={e => setForm(f => ({ ...f, [key]: e.target.value }))} placeholder={placeholder}
                  style={{ width: '100%', padding: '0.6rem 0.75rem', borderRadius: 6, background: 'oklch(0.18 0 0)', border: '1px solid oklch(0.28 0 0)', color: 'oklch(0.985 0 0)', fontSize: 13, boxSizing: 'border-box' }} />
              </div>
            ))}
            <div>
              <label style={{ display: 'block', fontSize: 12, color: 'oklch(0.6 0 0)', marginBottom: 6 }}>Environment</label>
              <select value={form.environment} onChange={e => setForm(f => ({ ...f, environment: e.target.value }))}
                style={{ padding: '0.6rem 0.75rem', borderRadius: 6, background: 'oklch(0.18 0 0)', border: '1px solid oklch(0.28 0 0)', color: 'oklch(0.985 0 0)', fontSize: 13 }}>
                <option value="development">Development</option>
                <option value="staging">Staging</option>
                <option value="production">Production</option>
              </select>
            </div>
            <div>
              <label style={{ display: 'block', fontSize: 12, color: 'oklch(0.6 0 0)', marginBottom: 8 }}>Auto-provision</label>
              <div style={{ display: 'flex', gap: '1rem', flexWrap: 'wrap' }}>
                {[{ key: 'repo', label: '🐙 GitHub Repo' }, { key: 'cicd', label: '⚡ CI/CD Pipeline' }, { key: 'k8s', label: '☸ K8s Namespace' }].map(({ key, label }) => (
                  <label key={key} style={{ display: 'flex', alignItems: 'center', gap: '0.5rem', cursor: 'pointer', fontSize: 13, color: 'oklch(0.75 0 0)' }}>
                    <input type="checkbox" checked={(form as any)[key]} onChange={e => setForm(f => ({ ...f, [key]: e.target.checked }))} />
                    {label}
                  </label>
                ))}
              </div>
            </div>
          </div>
          <div style={{ display: 'flex', gap: '0.75rem', justifyContent: 'flex-end', marginTop: '1.5rem' }}>
            <button onClick={() => setStep(0)} style={{ padding: '0.6rem 1.25rem', borderRadius: 8, background: 'transparent', color: 'oklch(0.6 0 0)', border: '1px solid oklch(0.28 0 0)', cursor: 'pointer', fontSize: 14 }}>← Back</button>
            <button disabled={!form.name} onClick={() => setStep(2)} style={{ padding: '0.6rem 1.5rem', borderRadius: 8, background: form.name ? 'oklch(0.6 0.2 250)' : 'oklch(0.25 0 0)', color: form.name ? '#fff' : 'oklch(0.45 0 0)', border: 'none', cursor: form.name ? 'pointer' : 'not-allowed', fontWeight: 600, fontSize: 14 }}>Next →</button>
          </div>
        </div>
      )}

      {/* Step 2: Review */}
      {step === 2 && (
        <div style={{ background: 'oklch(0.14 0 0)', border: '1px solid oklch(0.22 0 0)', borderRadius: 10, padding: '1.5rem' }}>
          <h3 style={{ color: 'oklch(0.985 0 0)', margin: '0 0 1.25rem', fontSize: 16 }}>Review & Create</h3>
          <div style={{ display: 'grid', gap: '0.75rem', marginBottom: '1.5rem' }}>
            {[
              { label: 'Template', value: templates.find(t => t.id === selected)?.name },
              { label: 'Service Name', value: form.name },
              { label: 'Description', value: form.description || '—' },
              { label: 'Team', value: form.team || '—' },
              { label: 'Environment', value: form.environment },
              { label: 'Auto-provision', value: [form.repo && 'GitHub Repo', form.cicd && 'CI/CD', form.k8s && 'K8s'].filter(Boolean).join(', ') || 'None' },
            ].map(({ label, value }) => (
              <div key={label} style={{ display: 'flex', gap: '1rem', padding: '0.6rem 0', borderBottom: '1px solid oklch(0.2 0 0)' }}>
                <span style={{ color: 'oklch(0.55 0 0)', fontSize: 13, minWidth: 130 }}>{label}</span>
                <span style={{ color: 'oklch(0.985 0 0)', fontSize: 13, fontWeight: 500 }}>{value}</span>
              </div>
            ))}
          </div>
          <div style={{ display: 'flex', gap: '0.75rem', justifyContent: 'flex-end' }}>
            <button onClick={() => setStep(1)} style={{ padding: '0.6rem 1.25rem', borderRadius: 8, background: 'transparent', color: 'oklch(0.6 0 0)', border: '1px solid oklch(0.28 0 0)', cursor: 'pointer', fontSize: 14 }}>← Back</button>
            <button onClick={handleCreate} disabled={loading} style={{ padding: '0.6rem 1.5rem', borderRadius: 8, background: 'oklch(0.6 0.18 150)', color: '#fff', border: 'none', cursor: 'pointer', fontWeight: 600, fontSize: 14, display: 'flex', alignItems: 'center', gap: '0.5rem' }}>
              {loading ? '⏳ Creating...' : <><Rocket size={15} /> Create Service</>}
            </button>
          </div>
        </div>
      )}
    </div>
  )
}
