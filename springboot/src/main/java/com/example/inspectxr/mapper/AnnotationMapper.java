package com.example.inspectxr.mapper;

import com.example.inspectxr.dtos.*;
import com.example.inspectxr.entity.*;
import org.mapstruct.*;

@Mapper(componentModel = "spring")
public interface AnnotationMapper {

    PointAnnotationEntity toPointEntity(PointAnnotationRequestDTO dto);

    @Mapping(target = "createdById", source = "createdBy.id")
    @Mapping(target = "modelId", source = "model.id")
    @Mapping(target = "sessionId", source = "session.id")
    PointAnnotationResponseDTO toPointDTO(PointAnnotationEntity entity);

    PaintAnnotationEntity toPaintEntity(PaintAnnotationRequestDTO dto);

    @Mapping(target = "createdById", source = "createdBy.id")
    @Mapping(target = "modelId", source = "model.id")
    @Mapping(target = "sessionId", source = "session.id")
    PaintAnnotationResponseDTO toPaintDTO(PaintAnnotationEntity entity);
}
