package com.example.inspectxr.service;

import com.example.inspectxr.dtos.*;
import com.example.inspectxr.entity.*;
import com.example.inspectxr.mapper.AnnotationMapper;
import com.example.inspectxr.repository.*;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.HttpStatus;
import org.springframework.stereotype.Service;
import org.springframework.web.server.ResponseStatusException;

import java.util.List;
import java.util.stream.Collectors;

@Service
public class AnnotationService {

    private final AnnotationRepository annotationRepository;
    private final PointAnnotationRepository pointAnnotationRepository;
    private final PaintAnnotationRepository paintAnnotationRepository;
    private final UserRepository userRepository;
    private final ModelRepository modelRepository;
    private final SessionRepository sessionRepository;
    private final AnnotationMapper annotationMapper;

    @Autowired
    public AnnotationService(AnnotationRepository annotationRepository,
                             PointAnnotationRepository pointAnnotationRepository,
                             PaintAnnotationRepository paintAnnotationRepository,
                             UserRepository userRepository,
                             ModelRepository modelRepository,
                             SessionRepository sessionRepository,
                             AnnotationMapper annotationMapper) {
        this.annotationRepository = annotationRepository;
        this.pointAnnotationRepository = pointAnnotationRepository;
        this.paintAnnotationRepository = paintAnnotationRepository;
        this.userRepository = userRepository;
        this.modelRepository = modelRepository;
        this.sessionRepository = sessionRepository;
        this.annotationMapper = annotationMapper;
    }

    private void populateCommonFields(AnnotationEntity entity, Long createdById, Long modelId, Long sessionId) {
        UserEntity user = userRepository.findById(createdById)
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND, "User not found"));
        ModelEntity model = modelRepository.findById(modelId)
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND, "Model not found"));
        SessionEntity session = sessionRepository.findById(sessionId)
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND, "Session not found"));

        entity.setCreatedBy(user);
        entity.setModel(model);
        entity.setSession(session);
    }

    public PointAnnotationResponseDTO getPointAnnotationById(Long id) {
        PointAnnotationEntity entity = pointAnnotationRepository.findById(id)
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND, "Point annotation not found"));
        return annotationMapper.toPointDTO(entity);
    }

    public PaintAnnotationResponseDTO getPaintAnnotationById(Long id) {
        PaintAnnotationEntity entity = paintAnnotationRepository.findById(id)
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND, "Paint annotation not found"));
        return annotationMapper.toPaintDTO(entity);
    }


    public AnnotationResponseDTO addPointAnnotation(PointAnnotationRequestDTO dto) {
        PointAnnotationEntity entity = annotationMapper.toPointEntity(dto);
        populateCommonFields(entity, dto.getCreatedById(), dto.getModelId(), dto.getSessionId());
        PointAnnotationEntity saved = pointAnnotationRepository.save(entity);
        return annotationMapper.toPointDTO(saved);
    }

    public AnnotationResponseDTO addPaintAnnotation(PaintAnnotationRequestDTO dto) {
        PaintAnnotationEntity entity = annotationMapper.toPaintEntity(dto);
        populateCommonFields(entity, dto.getCreatedById(), dto.getModelId(), dto.getSessionId());
        PaintAnnotationEntity saved = paintAnnotationRepository.save(entity);
        return annotationMapper.toPaintDTO(saved);
    }

    public AnnotationResponseDTO updatePointAnnotation(Long id, PointAnnotationRequestDTO dto) {
        PointAnnotationEntity existing = pointAnnotationRepository.findById(id)
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND, "Point annotation not found"));

        existing.setPosX(dto.getPosX());
        existing.setPosY(dto.getPosY());
        existing.setPosZ(dto.getPosZ());
        existing.setData(dto.getData());

        PointAnnotationEntity saved = pointAnnotationRepository.save(existing);
        return annotationMapper.toPointDTO(saved);
    }

    public AnnotationResponseDTO updatePaintAnnotation(Long id, PaintAnnotationRequestDTO dto) {
        PaintAnnotationEntity existing = paintAnnotationRepository.findById(id)
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND, "Paint annotation not found"));

        existing.setImageLayer(dto.getImageLayer());
        existing.setColor(dto.getColor());
        existing.setData(dto.getData());

        PaintAnnotationEntity saved = paintAnnotationRepository.save(existing);
        return annotationMapper.toPaintDTO(saved);
    }

    public void deleteAnnotation(Long id) {
        if (!annotationRepository.existsById(id)) {
            throw new ResponseStatusException(HttpStatus.NOT_FOUND, "Annotation not found");
        }
        annotationRepository.deleteById(id);
    }

    public List<PointAnnotationResponseDTO> getPointAnnotationsBySession(Long sessionId) {
        List<PointAnnotationEntity> list = pointAnnotationRepository.findAllBySessionId(sessionId);
        return list.stream().map(annotationMapper::toPointDTO).collect(Collectors.toList());
    }

    public List<PaintAnnotationResponseDTO> getPaintAnnotationsBySession(Long sessionId) {
        List<PaintAnnotationEntity> list = paintAnnotationRepository.findAllBySessionId(sessionId);
        return list.stream().map(annotationMapper::toPaintDTO).collect(Collectors.toList());
    }
}
